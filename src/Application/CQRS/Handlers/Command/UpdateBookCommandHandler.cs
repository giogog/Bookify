using Application.MediatR.Commands;
using Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Command
{
    public class UpdateBookCommandHandler(IRepositoryManager repositoryManager, ILogger<UpdateBookCommandHandler> logger):IRequestHandler<UpdateBookCommand>
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly ILogger<UpdateBookCommandHandler> _logger = logger;

        public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating Book on Id {request.BookId}");

            var book = await _repositoryManager.BookRepository.GetBookByIdAsync(request.BookId);

            if (book == null)
            {
                _logger.LogError($"Book with Id {request.BookId} not found");
                throw new NotFoundException("Book not found");
            }

            book.Name = request.Name;
            book.Price = request.Price;

            // Try to save the new photo
            try
            {
                _repositoryManager.BookRepository.UpdateBook(book);
                await _repositoryManager.SaveAsync();

                _logger.LogInformation("Successfully Updated book '{BookName}'.", book.Id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update book '{BookName}'.", book.Id.ToString());
                throw; // Re-throw the exception after logging
            }
        }
    }
}
