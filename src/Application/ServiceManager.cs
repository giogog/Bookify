using Application.Services;
using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthorizationService> _authorizationService;
    private readonly Lazy<IAuthorizationEmailService> _emailService;
    private readonly Lazy<IWishlistService> _wishlistService;
    private readonly Lazy<IPhotoService> _photoService;
    public ServiceManager(
        IRepositoryManager repositoryManager,
        UserManager<User> userManager,
        ITokenGenerator tokenGenerator,
        IEmailSender emailSender,
        ILoggerFactory loggerFactory,
        IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryOptions
        )
    {
        _authorizationService = new (() => new AuthorizationService(tokenGenerator, repositoryManager, loggerFactory.CreateLogger<AuthorizationService>()));
        _emailService = new (() => new AuthorizationEmailService(repositoryManager, loggerFactory.CreateLogger<AuthorizationEmailService>()));
        _wishlistService = new (() => new WishlistService(repositoryManager, loggerFactory.CreateLogger<WishlistService>(), mapper));
        _photoService = new(() => new PhotoService(cloudinaryOptions, loggerFactory.CreateLogger<PhotoService>()));

    }
    public IPhotoService PhotoService => _photoService.Value;
    public IAuthorizationService AuthorizationService => _authorizationService.Value;
    public IAuthorizationEmailService EmailService => _emailService.Value;
    public IWishlistService WishlistService => _wishlistService.Value;
}
