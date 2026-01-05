using Application.MediatR.Queries;
using MediatR;
using System.Net;

namespace API.Category;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/Categories", async (IMediator mediator) =>
        {
            var categories = await mediator.Send(new GetCategoriesQuery());

            var apiResponse = new ApiResponse("Categories", true, categories, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });
    }
}
