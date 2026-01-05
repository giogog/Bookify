using Application.MediatR.Notifications;
using Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

public class SendConfirmationMailEventHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly IEmailSender _emailSender;
    private readonly IRepositoryManager _repositoryManager;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<SendConfirmationMailEventHandler> _logger;

    public SendConfirmationMailEventHandler(
        IEmailSender emailSender,
        IRepositoryManager repositoryManager,
        ITokenGenerator tokenGenerator,
        ILogger<SendConfirmationMailEventHandler> logger)
    {
        _emailSender = emailSender;
        _repositoryManager = repositoryManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending confirmation email for user {Username}.", notification.Username);

        var user = await _repositoryManager.UserRepository.GetUserAsync(u => u.UserName == notification.Username);
        if (user == null)
        {
            _logger.LogWarning("User {Username} not found.", notification.Username);
            return;
        }

        var token = await _tokenGenerator.GenerateMailTokenCode(user);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Failed to generate token for user {Username}.", notification.Username);
            return;
        }

        var baseUrl = (notification.BaseUrl ?? string.Empty).TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            _logger.LogWarning("Base URL not provided for user {Username}.", notification.Username);
            return;
        }

        var callbackUrl = $"{baseUrl}/api/Account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        _logger.LogInformation("Sending confirmation email to {Email} for user {Username}.", user.Email, notification.Username);

        var emailResult = await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
        if (!emailResult.IsSuccess)
        {
            _logger.LogError("Failed to send confirmation email to {Email} for user {Username}: {ErrorMessage}.", user.Email, notification.Username, emailResult.ErrorMessage);
            throw new MailNotSend($"Sending Confirmation mail failed: {emailResult.ErrorMessage}");
        }
    }
}
