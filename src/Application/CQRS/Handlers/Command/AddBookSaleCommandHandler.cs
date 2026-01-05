using Application.MediatR.Commands;
using Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Command;

public class AddBookSaleCommandHandler(IRepositoryManager repositoryManager, ILogger<AddBookSaleCommandHandler> logger) : IRequestHandler<AddBookSaleCommand>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<AddBookSaleCommandHandler> _logger = logger;

    public async Task Handle(AddBookSaleCommand request, CancellationToken cancellationToken)
    {

        _logger.LogInformation($"Updating Sale on book with Id: {request.BookId.ToString()}.");
        var book = await _repositoryManager.BookRepository.GetBookByIdAsync(request.BookId);

        if (book == null) 
        {
            throw new NotFoundException($"Book on this id: {request.BookId.ToString()} not Found ");
        }

        book.Sale = request.Percentage;

        _repositoryManager.BookRepository.UpdateBook(book);
        // Try to save changes
        try
        {
            await _repositoryManager.SaveAsync();
            _logger.LogInformation("Successfully saved sale update.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update book for BookId: {BookId}.", request.BookId);
            throw; 
        }

    }
}
