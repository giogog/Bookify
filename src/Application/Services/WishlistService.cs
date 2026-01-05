using Application.Extensions;
using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class WishlistService(IRepositoryManager repositoryManager, ILogger<WishlistService> logger,IMapper mapper):IWishlistService
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<WishlistService> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task AddBookToWishlist(string userId, string bookId)
    {
        _logger.LogInformation("Attempting to add book {BookId} to wishlist for user {UserId}", bookId, userId);

        var wishlistEntry = new Wishlist
        {
            UserId = userId.GetGuid(),
            BookId = bookId.GetGuid(),
            AddedAt = DateTime.UtcNow
        };

        try
        {
            await _repositoryManager.WishlistRepository.CreateWishlistAsync(wishlistEntry);
            await _repositoryManager.SaveAsync();
            _logger.LogInformation("Successfully added book {BookId} to wishlist for user {UserId}", bookId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add book {BookId} to wishlist for user {UserId}", bookId, userId);
            throw; 
        }
    }

    public async Task RemoveBookFromWishlist(string userId, string bookId)
    {
        _logger.LogInformation("Attempting to remove book {BookId} from wishlist for user {UserId}", bookId, userId);

        var wishlistEntry = await _repositoryManager.WishlistRepository.Wishlists()
            .FirstOrDefaultAsync(w => w.UserId == userId.GetGuid() && w.BookId == bookId.GetGuid());

        if (wishlistEntry == null)
        {
            _logger.LogWarning("Wishlist entry not found for user {UserId} and book {BookId}", userId, bookId);
            throw new NotFoundException("Wishlist entry not found for user ");
        }

        try
        {
            _repositoryManager.WishlistRepository.DeleteWishlist(wishlistEntry);
            await _repositoryManager.SaveAsync();
            _logger.LogInformation("Successfully removed book {BookId} from wishlist for user {UserId}", bookId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove book {BookId} from wishlist for user {UserId}", bookId, userId);
            throw;
        }
    }

    public async Task<List<BookDto>> GetWishlistForUser(string userId)
    {
        _logger.LogInformation("Fetching wishlist for user {UserId}", userId);

        var wishlist = await _repositoryManager.WishlistRepository.Wishlists()
            .AsNoTracking()
            .Where(w => w.UserId == userId.GetGuid())
            .Select(w => w.Book == null
                ? null
                : new BookDto(
                    w.Book.Name,
                    w.Book.Price,
                    w.Book.Ratings != null && w.Book.Ratings.Any() ? Math.Round(w.Book.Ratings.Average(r => (double)r.Stars), 1) : 0.0,
                    w.Book.Author != null ? w.Book.Author.Name : string.Empty,
                    w.Book.Author != null ? w.Book.Author.Surname : null,
                    w.Book.Category != null ? w.Book.Category.Name : string.Empty,
                    w.Book.SalePrice,
                    w.Book.Sale,
                    w.Book.Photo != null ? w.Book.Photo.Url : null))
            .Where(dto => dto != null)
            .Select(dto => dto!)
            .ToListAsync();


        if (!wishlist.Any())
        {
            _logger.LogInformation("No wishlist items found for user {UserId}", userId);
            throw new NotFoundException("No wishlist items found for user");
        }
        return wishlist;
        //turn _mapper.Map<List<BookDto>>(wishlist);
    }

    
}
