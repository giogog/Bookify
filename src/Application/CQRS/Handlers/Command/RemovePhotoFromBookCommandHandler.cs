using Application.CQRS.Commands;
using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.CQRS.Handlers.Command;

public class RemovePhotoFromBookCommandHandler(IRepositoryManager repositoryManager, IServiceManager serviceManager, ILogger<RemovePhotoFromBookCommandHandler> logger):IRequestHandler<RemovePhotoFromBookCommand>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly IServiceManager _serviceManager = serviceManager;
    private readonly ILogger<RemovePhotoFromBookCommandHandler> _logger = logger;

    public async Task Handle(RemovePhotoFromBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deleting photo Book on Id {request.BookId}");

        var photo = await _repositoryManager.BookRepository.Books().Where(b=>b.Id == request.BookId).Select(b=>b.Photo).FirstOrDefaultAsync();

        if (photo == null)
        {
            _logger.LogError($"photo on book with Id {request.BookId} not found");
            throw new NotFoundException("Photo not found");
        }
        var deleteResult = await _serviceManager.PhotoService.DeletePhotoAsync(photo.PublicId);


        if (deleteResult.Error != null)
        {
            _logger.LogError(deleteResult.Error.Message);
            throw new Exception("Couldn't delete photo on Cloudinary");
        }
        else
        {
            try
            {
                _repositoryManager.BookRepository.DeletePhoto(photo);
                await _repositoryManager.SaveAsync();

                _logger.LogInformation("Successfully deleted photo to book with id '{BookName}'.", request.BookId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Delete photo on book with id '{BookName}'.", request.BookId.ToString());
                throw; // Re-throw the exception after logging
            }
        }

    }
}