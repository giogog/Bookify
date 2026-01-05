using Domain.Entities;
using Domain.Models;

namespace Contracts;

public interface IWishlistService
{
    Task AddBookToWishlist(string userId, string bookId);
    Task RemoveBookFromWishlist(string userId, string bookId);
    Task<List<BookDto>> GetWishlistForUser(string userId);
}
