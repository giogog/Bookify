using Application.CQRS.Commands;
using Application.MediatR.Handlers.Command;
using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.CQRS.Handlers.Command;

public class AddPhotoToBookCommandHandler(IRepositoryManager repositoryManager, IServiceManager serviceManager, ILogger<AddPhotoToBookCommandHandler> logger):IRequestHandler<AddPhotoToBookCommand>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly IServiceManager _serviceManager = serviceManager;
    private readonly ILogger<AddPhotoToBookCommandHandler> _logger = logger;

    public async Task Handle(AddPhotoToBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _repositoryManager.BookRepository.GetBookByIdAsync(request.BookId);

        if (book == null)
        {
            throw new NotFoundException("Book on this id not found");
        }

        var uploadResult = await _serviceManager.PhotoService.AddPhotoAsync(request.Photo);

        if (uploadResult.Error != null)
        {
            _logger.LogError(uploadResult.Error.Message);
            throw new Exception("Couldn't upload photo on Cloudinary");
        }
        else
        {
            book.Photo = new Photo
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };

            try
            {
                _repositoryManager.BookRepository.UpdateBook(book);
                await _repositoryManager.SaveAsync();

                _logger.LogInformation("Successfully Uploaded phpto to book with id '{BookName}'.", book.Id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Upload photo on book with id '{BookName}'.", book.Id.ToString());
                throw; // Re-throw the exception after logging
            }


        }

    }
}
