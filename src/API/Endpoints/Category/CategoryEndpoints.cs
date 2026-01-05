using Application.MediatR.Queries;
using MediatR;
using System.Net;

namespace API.Endpoints.Category;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/Categories", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var categories = await mediator.Send(new GetCategoriesQuery(), cancellationToken);

            var apiResponse = new ApiResponse("Categories", true, categories, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });
    }
}
