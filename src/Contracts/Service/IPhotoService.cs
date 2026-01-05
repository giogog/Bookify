using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Contracts;

public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
