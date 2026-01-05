using Application.MediatR.Notifications;
using Contracts;
using Domain.Models;
using MediatR;
using System.Net;

namespace API.Account;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/Account");

        group.MapPost("/register", async (
            RegisterDto registerDto,
            IServiceManager serviceManager,
            IMediator mediator,
            HttpContext httpContext) =>
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
            await mediator.Publish(new UserCreatedNotification(registerDto.Username, baseUrl));

            var apiResponse = new ApiResponse(
                "Registration successful. Please check your email to confirm your account.",
                true,
                null,
                Convert.ToInt32(HttpStatusCode.Created));

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapPost("/login", async (LoginDto loginDto, IServiceManager serviceManager) =>
        {
            var loginCheckUp = await serviceManager.AuthorizationService.Login(loginDto);

            if (!loginCheckUp.Succeeded)
            {
                var response = new ApiResponse(
                    loginCheckUp.Errors.First().Description,
                    false,
                    null,
                    Convert.ToInt32(HttpStatusCode.BadRequest));

                return Results.Json(response, statusCode: response.StatusCode);
            }

            var apiResponse = new ApiResponse(
                "User logged in successfully",
                true,
                await serviceManager.AuthorizationService.Authenticate(user => user.UserName == loginDto.Username),
                Convert.ToInt32(HttpStatusCode.OK));

            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapGet("/confirm-email", async (int userId, string token, IServiceManager serviceManager) =>
        {
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
            HttpContext httpContext) =>
        {
            var alreadyConfirmed = await serviceManager.EmailService.CheckMailConfirmationAsync(username);

            if (alreadyConfirmed.IsSuccess)
            {
                var response = new ApiResponse(alreadyConfirmed.ErrorMessage, false, null, Convert.ToInt32(HttpStatusCode.BadRequest));
                return Results.Json(response, statusCode: response.StatusCode);
            }

            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            await mediator.Publish(new UserCreatedNotification(username, baseUrl));

            var apiResponse = new ApiResponse("Please check your email to confirm your account.", true, null, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapPost("/request-password-reset/{email}", async (
            string email,
            IMediator mediator,
            HttpContext httpContext) =>
        {
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            await mediator.Publish(new PasswordResetRequestNotification(email, baseUrl));

            var apiResponse = new ApiResponse("Please check your email to for password reset.", true, null, Convert.ToInt32(HttpStatusCode.Created));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        group.MapGet("/reset-password-token", (string token, string email) =>
        {
            var apiResponse = new ApiResponse("Password reset Token.", true, token, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
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
