using Application.MediatR.Notifications;
using Contracts;
using Domain.Models;
using MediatR;
using System.Net;

namespace API.Endpoints.Account;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/Account");

        group.MapPost("/register", async (
            RegisterDto registerDto,
            IServiceManager serviceManager,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var registrationCheckUp = await serviceManager.AuthorizationService.Register(registerDto);

            if (!registrationCheckUp.Succeeded)
            {
                var response = new ApiResponse(
                    registrationCheckUp.Errors.First().Description,
                    false,
                    null,
                    Convert.ToInt32(HttpStatusCode.BadRequest));

                return Results.Json(response, statusCode: response.StatusCode);
            }

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            await mediator.Publish(new UserCreatedNotification(registerDto.Username, baseUrl), cancellationToken);

            var apiResponse = new ApiResponse(
                "Registration successful. Please check your email to confirm your account.",
                true,
                null,
                Convert.ToInt32(HttpStatusCode.Created));

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapPost("/login", async (LoginDto loginDto, IServiceManager serviceManager) =>
        {
            var result = await serviceManager.AuthorizationService.LoginWithToken(loginDto);

            if (!result.Succeeded)
            {
                // Avoid user enumeration by using the same message for missing user vs wrong password.
                var statusCode = string.Equals(result.ErrorCode, "InvalidCredentials", StringComparison.Ordinal)
                    ? Convert.ToInt32(HttpStatusCode.Unauthorized)
                    : Convert.ToInt32(HttpStatusCode.BadRequest);

                var failureResponse = new ApiResponse(
                    result.ErrorMessage ?? "Login failed.",
                    false,
                    null,
                    statusCode);

                return Results.Json(failureResponse, statusCode: failureResponse.StatusCode);
            }

            var apiResponse = new ApiResponse(
                "User logged in successfully",
                true,
                result.Data,
                Convert.ToInt32(HttpStatusCode.OK));

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapGet("/confirm-email", async (Guid userId, string token, IServiceManager serviceManager) =>
        {
            // Token may contain '+' which can be decoded as a space in query strings.
            token = token.Replace(' ', '+');

            var result = await serviceManager.EmailService.ConfirmEmailAsync(userId.ToString(), token);

            if (!result.Succeeded)
            {
                var response = new ApiResponse("Unable to Confirm Mail", false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
                return Results.Json(response, statusCode: response.StatusCode);
            }

            var apiResponse = new ApiResponse("Mail Confirmed Successfully", true, null, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapPost("/resend-confirmation/{username}", async (
            string username,
            IServiceManager serviceManager,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var alreadyConfirmed = await serviceManager.EmailService.CheckMailConfirmationAsync(username);

            if (alreadyConfirmed.IsSuccess)
            {
                var response = new ApiResponse(alreadyConfirmed.ErrorMessage, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
                return Results.Json(response, statusCode: response.StatusCode);
            }

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            await mediator.Publish(new UserCreatedNotification(username, baseUrl), cancellationToken);

            var apiResponse = new ApiResponse("Please check your email to confirm your account.", true, null, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        static async Task<IResult> RequestPasswordReset(
            string email,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            await mediator.Publish(new PasswordResetRequestNotification(email, baseUrl), cancellationToken);

            var apiResponse = new ApiResponse("Please check your email to for password reset.", true, null, Convert.ToInt32(HttpStatusCode.Created));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        }

        // Preferred: do not place emails in URLs.
        group.MapPost("/request-password-reset", async (
            PasswordResetRequestDto request,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            return await RequestPasswordReset(request.Email, mediator, httpContext, cancellationToken);
        });

        // Backward-compatible: keep the existing route.
        group.MapPost("/request-password-reset/{email}", async (
            string email,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            return await RequestPasswordReset(email, mediator, httpContext, cancellationToken);
        });

        group.MapPut("/reset-password", async (ResetPasswordDto resetPasswordDto, IServiceManager serviceManager) =>
        {
            var result = await serviceManager.AuthorizationService.ResetPassword(resetPasswordDto);

            if (!result.Succeeded)
            {
                var response = new ApiResponse(result.Errors.First().Description, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
                return Results.Json(response, statusCode: response.StatusCode);
            }

            var apiResponse = new ApiResponse("Password reset success.", true, null, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });
    }
}
