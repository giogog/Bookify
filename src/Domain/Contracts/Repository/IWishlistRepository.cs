using Domain.Entities;

namespace Contracts;

public interface IWishlistRepository
{
    Task CreateWishlistAsync(Wishlist wishlist);
    void DeleteWishlist(Wishlist wishlist);
    void UpdateWishlist(Wishlist wishlist);
    IQueryable<Wishlist> Wishlists();

}
