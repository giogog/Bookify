using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(
            ITokenGenerator tokenGenerator,
            IRepositoryManager repositoryManager,
            ILogger<AuthorizationService> logger)
        {
            _tokenGenerator = tokenGenerator;
            _repositoryManager = repositoryManager;
            _logger = logger;
        }

        public async Task<LoginResponseDto> Authenticate(Expression<Func<User, bool>> expression)
        {
            _logger.LogInformation("Authenticating user.");

            var user = await _repositoryManager.UserRepository.GetUserAsync(expression);
            if (user == null)
            {
                _logger.LogWarning("User not found for the provided expression.");
                throw new NotFoundException("User not found.");
            }

            var username = user.UserName ?? throw new InvalidOperationException("UserName is missing for the authenticated user.");

            _logger.LogInformation("User {Username} found. Generating token.", username);

            var token = await _tokenGenerator.GenerateToken(user);

            _logger.LogInformation("Token generated for user {Username}.", username);

            return new LoginResponseDto(user.Id, username, token);
        }

        public async Task<IdentityResult> Register(RegisterDto registerDto)
        {
            _logger.LogInformation("Registering user {Username}.", registerDto.Username);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var result = await _repositoryManager.UserRepository.CreateUserAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("User registration failed for {Username}: {Errors}", registerDto.Username, result.Errors);
                    await _repositoryManager.RollbackTransactionAsync();
                    return result;
                }

                var roleAssignmentResult = await AssignRoleToUser(user, "User");
                if (!roleAssignmentResult.Succeeded)
                {
                    await _repositoryManager.RollbackTransactionAsync();
                    return roleAssignmentResult;
                }

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("User {Username} registered successfully.", registerDto.Username);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for {Username}", registerDto.Username);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IdentityResult> Login(LoginDto loginDto)
        {
            _logger.LogInformation("Logging in user {Username}.", loginDto.Username);

            var user = await _repositoryManager.UserRepository.GetUserAsync(user => user.UserName == loginDto.Username);

            if (user == null)
            {
                _logger.LogWarning("User {Username} does not exist.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "UserDoesNotExist", Description = "The user does not exist." });
            }

            var passwordCheck = await _repositoryManager.UserRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCheck)
            {
                _logger.LogWarning("Incorrect password for user {Username}.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "IncorrectPassword", Description = "Incorrect password." });
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email not confirmed for user {Username}.", loginDto.Username);
                return IdentityResult.Failed(new IdentityError { Code = "MailIsNotConfirmed", Description = "Please confirm your email." });
            }

            _logger.LogInformation("User {Username} logged in successfully.", loginDto.Username);

            return IdentityResult.Success;
        }

        public async Task<LoginWithTokenResult> LoginWithToken(LoginDto loginDto)
        {
            _logger.LogInformation("Logging in user {Username}.", loginDto.Username);

            var user = await _repositoryManager.UserRepository.GetUserAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                _logger.LogWarning("User {Username} does not exist.", loginDto.Username);
                // Avoid user enumeration.
                return new LoginWithTokenResult(false, "InvalidCredentials", "Invalid username or password.", null);
            }

            var passwordCheck = await _repositoryManager.UserRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCheck)
            {
                _logger.LogWarning("Incorrect password for user {Username}.", loginDto.Username);
                return new LoginWithTokenResult(false, "InvalidCredentials", "Invalid username or password.", null);
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email not confirmed for user {Username}.", loginDto.Username);
                return new LoginWithTokenResult(false, "MailNotConfirmed", "Please confirm your email.", null);
            }

            var username = user.UserName ?? throw new InvalidOperationException("UserName is missing for the authenticated user.");

            var token = await _tokenGenerator.GenerateToken(user);

            _logger.LogInformation("User {Username} logged in successfully.", username);

            return new LoginWithTokenResult(true, null, null, new LoginResponseDto(user.Id, username, token));
        }

        public async Task<IdentityResult> AddNewRole(string newRole)
        {
            _logger.LogInformation("Adding new role {Role}.", newRole);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                if (await _repositoryManager.RoleRepository.UserRoleExists(newRole))
                {
                    _logger.LogWarning("Role {Role} already exists.", newRole);
                    await _repositoryManager.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Code = "AlreadyExists", Description = "Role already exists." });
                }

                var role = new Role
                {
                    Name = newRole,
                    NormalizedName = newRole.ToUpper(),
                };

                var result = await _repositoryManager.RoleRepository.CreateUserRoleAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create role {Role}: {Errors}", newRole, result.Errors);
                    await _repositoryManager.RollbackTransactionAsync();
                    return result;
                }

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("Role {Role} created successfully.", newRole);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role {Role}", newRole);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Resetting password for user with email {Email}.", resetPasswordDto.Email);

            await _repositoryManager.BeginTransactionAsync();

            try
            {
                var user = await _repositoryManager.UserRepository.GetUserAsync(u => u.Email == resetPasswordDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found.", resetPasswordDto.Email);
                    await _repositoryManager.RollbackTransactionAsync();
                    throw new NotFoundException("User not found.");
                }

                var result = await _repositoryManager.UserRepository.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to reset password for user {Email}: {Errors}", resetPasswordDto.Email, result.Errors);
                    await _repositoryManager.RollbackTransactionAsync();
                    return result;
                }

                await _repositoryManager.CommitTransactionAsync();

                _logger.LogInformation("Password reset successfully for user {Email}.", resetPasswordDto.Email);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting password for user {Email}", resetPasswordDto.Email);
                await _repositoryManager.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task<IdentityResult> AssignRoleToUser(User user, string roleName)
        {
            _logger.LogInformation("Assigning role {Role} to user {Username}.", roleName, user.UserName);

            if (!await _repositoryManager.RoleRepository.UserRoleExists(roleName))
            {
                _logger.LogError("Role {Role} does not exist.", roleName);
                return IdentityResult.Failed(new IdentityError { Code = "RoleNotExists", Description = "Role does not exist." });
            }

            var result = await _repositoryManager.UserRepository.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to assign role {Role} to user {Username}: {Errors}", roleName, user.UserName, result.Errors);
            }
            else
            {
                _logger.LogInformation("Role {Role} assigned to user {Username} successfully.", roleName, user.UserName);
            }

            return result;
        }
    }
}
