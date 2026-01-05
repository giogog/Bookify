using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IOptions<CloudinarySettings> _options;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<PhotoService> _logger; 

        public PhotoService(IOptions<CloudinarySettings> options, ILogger<PhotoService> logger)
        {
            _logger = logger; 
            _options = options;

            var account = new Account(options.Value.CloudName, options.Value.ApiKey, options.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);

            _logger.LogInformation("PhotoService initialized with Cloudinary settings.");
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                _logger.LogInformation("Starting photo upload: {FileName}", file.FileName);

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "eLibrary"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                _logger.LogInformation("PhotoDir upload completed: {FileName}, Status: {Status}",
                    file.FileName, uploadResult.StatusCode);
            }
            else
            {
                _logger.LogWarning("PhotoDir {FileName} is empty. Upload not performed.", file.FileName);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            _logger.LogInformation("Starting photo deletion for PublicId: {PublicId}", publicId);

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            _logger.LogInformation("PhotoDir deletion completed for PublicId: {PublicId}, Status: {Status}",
                publicId, result.StatusCode);

            return result;
        }
    }
}
