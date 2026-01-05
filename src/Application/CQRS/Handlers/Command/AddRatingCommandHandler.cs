using Application.MediatR.Commands;
using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Command;

public class AddRatingCommandHandler(IRepositoryManager repositoryManager, ILogger<AddBookCommandHandler> logger) : IRequestHandler<AddRatingCommand>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<AddBookCommandHandler> _logger = logger;

    public async Task Handle(AddRatingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start handling AddRatingCommand for UserId: {UserId}, BookId: {BookId}", request.UserId, request.BookId);

        _logger.LogInformation("Checking if user and book exist.");

        var checkUserExistsTask = _repositoryManager.UserRepository.GetUserAsync(u => u.Id == request.UserId);
        var checkBookExistsTask = _repositoryManager.BookRepository.GetBookByIdAsync(request.BookId);

        var user = await checkUserExistsTask;
        var book = await checkBookExistsTask;

        // Log the user and book check results
        if (user == null)
        {
            _logger.LogWarning("User with ID '{UserId}' not found.", request.UserId);
            throw new NotFoundException("User not found");
        }
        else
        {
            _logger.LogInformation("User with ID '{UserId}' found.", request.UserId);
        }

        if (book == null)
        {
            _logger.LogWarning("Book with ID '{BookId}' not found.", request.BookId);
            throw new NotFoundException("Book not found");
        }
        else
        {
            _logger.LogInformation("Book with ID '{BookId}' found.", request.BookId);
        }

        // Check if a rating already exists for the user and book (EF-tracked)
        var rating = await _repositoryManager.RatingRepository
            .Ratings()
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.BookId == request.BookId, cancellationToken);

        if (rating != null)
        {
            // Update the existing rating
            _logger.LogInformation("Updating rating to {Stars} stars.", request.Star);
            rating.Stars = request.Star;
            _repositoryManager.RatingRepository.UpdateRating(rating);
        }
        else
        {
            // Create a new rating
            _logger.LogInformation("Creating a new rating with {Stars} stars.", request.Star);
            rating = new Rating() { BookId = request.BookId, UserId = request.UserId, Stars = request.Star };
            await _repositoryManager.RatingRepository.CreateRatingAsync(rating);
        }

        // Try to save changes
        try
        {
            await _repositoryManager.SaveAsync();
            _logger.LogInformation("Successfully saved rating.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save rating for UserId: {UserId}, BookId: {BookId}.", request.UserId, request.BookId);
            throw; // Re-throw the exception after logging
        }
    }

}
