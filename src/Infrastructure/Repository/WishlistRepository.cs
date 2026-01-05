using Contracts;
using Domain.Entities;
using Infrastructure.Context;

namespace Infrastructure.Repository;

public class WishlistRepository(ApplicationDbContext context) : BaseRepository<Wishlist>(context), IWishlistRepository
{
    public IQueryable<Wishlist> Wishlists() => FindAll();

    public async Task CreateWishlistAsync(Wishlist wishlist) => await Create(wishlist);

    public void DeleteWishlist(Wishlist wishlist) => Delete(wishlist);

    public void UpdateWishlist(Wishlist wishlist) => Update(wishlist);
}