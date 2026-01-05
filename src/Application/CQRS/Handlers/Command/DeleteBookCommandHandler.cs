using Application.MediatR.Commands;
using Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Command;

public class DeleteBookCommandHandler(IRepositoryManager repositoryManager, ILogger<DeleteBookCommandHandler> logger):IRequestHandler<DeleteBookCommand>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<DeleteBookCommandHandler> _logger = logger;

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deleting Book on Id {request.BookId}");

        var book = await _repositoryManager.BookRepository.GetBookByIdAsync(request.BookId);

        if (book == null)
        {
            _logger.LogError($"Book with Id {request.BookId} not found");
            throw new NotFoundException("Book not found");
        }

        // Try to save the new book
        try
        {
            _repositoryManager.BookRepository.DeleteBook(book);
            await _repositoryManager.SaveAsync();

            _logger.LogInformation("Successfully Deleted book '{BookName}'.", book.Id.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete book '{BookName}'.", book.Id.ToString());
            throw; // Re-throw the exception after logging
        }

    }
}
