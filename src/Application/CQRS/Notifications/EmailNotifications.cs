using Domain.Exception;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Application.MediatR.Notifications;



public record UserCreatedNotification(string Username, IUrlHelper UrlHelper) : INotification;


public record PasswordResetRequestNotification(string Email, IUrlHelper UrlHelper) : INotification;
