using Contracts;
using System.Net;
using System.Security.Claims;

namespace API.Endpoints.Wishlist;

public static class WishlistEndpoints
{
    public static void MapWishlistEndpoints(this IEndpointRouteBuilder app)
    {
        var userRoutes = app.MapGroup("api/User").RequireAuthorization("User_Policy");

        userRoutes.MapPost("/Wishlist/Add", async (string userId, string bookId, IServiceManager serviceManager, HttpContext httpContext) =>
        {
            var currentUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Results.Unauthorized();
            }

            await serviceManager.WishlistService.AddBookToWishlist(currentUserId, bookId);

            var apiResponse = new ApiResponse("Book successfully added to wishlish", true, null, Convert.ToInt32(HttpStatusCode.Created));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        userRoutes.MapDelete("/Wishlist/Remove", async (string userId, string bookId, IServiceManager serviceManager, HttpContext httpContext) =>
        {
            var currentUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Results.Unauthorized();
            }

            await serviceManager.WishlistService.RemoveBookFromWishlist(currentUserId, bookId);

            var apiResponse = new ApiResponse("Book successfully removed from wishlish", true, null, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });

        userRoutes.MapGet("/Wishlist/{userId}", async (string userId, IServiceManager serviceManager, HttpContext httpContext) =>
        {
            var currentUserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Results.Unauthorized();
            }

            var wishlist = await serviceManager.WishlistService.GetWishlistForUser(currentUserId);

            var apiResponse = new ApiResponse("User Wishlist", true, wishlist, Convert.ToInt32(HttpStatusCode.OK));
            return Results.Json(apiResponse, statusCode: apiResponse.StatusCode);
        });
    }
}
