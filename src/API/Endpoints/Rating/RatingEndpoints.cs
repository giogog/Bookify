using Application.MediatR.Commands;
using MediatR;
using System.Net;

namespace API.Endpoints.Rating;

public static class RatingEndpoints
{
    public static void MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var userRoutes = app.MapGroup("api/User").RequireAuthorization("User_Policy");

        userRoutes.MapPost("/Book/Rating", async (IMediator mediator, AddRatingCommand command) =>
        {
            await mediator.Send(command);

            var apiResponse = new ApiResponse("User rating on book is set", true, null, Convert.ToInt32(HttpStatusCode.Created));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });
    }
}
