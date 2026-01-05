using API.Endpoints.Account;
using API.Endpoints.Book;
using API.Endpoints.Category;
using API.Endpoints.Rating;
using API.Endpoints.Wishlist;

namespace API;

public static class MapEndpoints
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAccountEndpoints();
        app.MapBookEndpoints();
        app.MapCategoryEndpoints();
        app.MapRatingEndpoints();
        app.MapWishlistEndpoints();
    }
}
