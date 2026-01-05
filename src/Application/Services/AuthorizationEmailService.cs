using Contracts;
using Domain.Exception;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthorizationEmailService : Contracts.IAuthorizationEmailService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AuthorizationEmailService> _logger;

        public AuthorizationEmailService(IRepositoryManager repositoryManager, ILogger<AuthorizationEmailService> logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task<EmailResult> CheckMailConfirmationAsync(string username)
        {
            _logger.LogInformation("Checking email confirmation for user {Username}.", username);

            var user = await _repositoryManager.UserRepository.GetUserAsync(u => u.UserName == username);

            if (user == null)
            {
                _logger.LogWarning("User {Username} not found.", username);
                throw new NotFoundException("User not found.");
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogInformation("Email not confirmed for user {Username}.", username);
                return new EmailResult { IsSuccess = false };
            }

            _logger.LogInformation("Email confirmed for user {Username}.", username);

            return new EmailResult { IsSuccess = true, ErrorMessage = "Mail is Confirmed" };
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            _logger.LogInformation("Confirming email for user with ID {UserId}.", userId);

            var user = await _repositoryManager.UserRepository.GetUserAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "User not found." });
            }

            var result = await _repositoryManager.UserRepository.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to confirm email for user with ID {UserId}: {Errors}", userId, result.Errors);
            }
            else
            {
                _logger.LogInformation("Email confirmed successfully for user with ID {UserId}.", userId);
            }

            return result;
        }
    }
}
