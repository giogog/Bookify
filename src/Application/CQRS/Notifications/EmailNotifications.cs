using MediatR;

namespace Application.MediatR.Notifications;

public record UserCreatedNotification(string Username, string BaseUrl) : INotification;

public record PasswordResetRequestNotification(string Email, string BaseUrl) : INotification;
