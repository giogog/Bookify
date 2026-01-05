using Application.MediatR.Commands;
using MediatR;
using System.Net;
using System.Security.Claims;

namespace API.Endpoints.Rating;

public static class RatingEndpoints
{
    public static void MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var userRoutes = app.MapGroup("api/User").RequireAuthorization("User_Policy");

        userRoutes.MapPost("/Book/Rating", async (IMediator mediator, AddRatingCommand command, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var currentUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Results.Unauthorized();
            }

            var safeCommand = new AddRatingCommand(currentUserId, command.bid, command.stars);
            await mediator.Send(safeCommand, cancellationToken);

            var apiResponse = new ApiResponse("User rating on book is set", true, null, Convert.ToInt32(HttpStatusCode.Created));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });
    }
}
