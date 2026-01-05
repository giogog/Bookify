using Application.MediatR.Notifications;
using Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class SendPasswordResetEmailEventHandler : INotificationHandler<PasswordResetRequestNotification>
{
    private readonly IEmailSender _emailSender;
    private readonly IRepositoryManager _repositoryManager;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<SendPasswordResetEmailEventHandler> _logger;

    public SendPasswordResetEmailEventHandler(
        IEmailSender emailSender,
        IRepositoryManager repositoryManager,
        ITokenGenerator tokenGenerator,
        ILogger<SendPasswordResetEmailEventHandler> logger)
    {
        _emailSender = emailSender;
        _repositoryManager = repositoryManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task Handle(PasswordResetRequestNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending password reset email for user with email {Email}.", notification.Email);

        var user = await _repositoryManager.UserRepository.GetUserAsync(u => u.Email == notification.Email);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", notification.Email);
            throw new NotFoundException("User not found");
        }

        if (!user.EmailConfirmed)
        {
            _logger.LogWarning("Email is not confirmed for user with email {Email}.", notification.Email);
            throw new MailNotConfirmedException("Email is not confirmed");
        }

        var token = await _tokenGenerator.GeneratePasswordResetToken(user);

        var callbackUrl = notification.UrlHelper.Action("ResetPasswordToken", "Account", new { token, email = notification.Email }, protocol: "https");
        if (string.IsNullOrEmpty(callbackUrl))
        {
            _logger.LogWarning("Failed to generate callback URL for password reset for user with email {Email}.", notification.Email);
            return;
        }

        var emailResult = await _emailSender.SendEmailAsync(notification.Email, "Reset Password", $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");
        if (emailResult.IsSuccess)
        {
            _logger.LogInformation("Password reset email sent successfully to user with email {Email}.", notification.Email);
        }
        else
        {
            _logger.LogError("Failed to send password reset email to user with email {Email}: {ErrorMessage}.", notification.Email, emailResult.ErrorMessage);
            throw new MailNotSend($"Sending mail failed: {emailResult.ErrorMessage}");
        }
    }
}
