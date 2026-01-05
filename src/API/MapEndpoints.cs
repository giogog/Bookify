using API.Account;
using API.Book;
using API.Category;
using API.Rating;
using API.Wishlist;

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
