namespace Contracts;

public interface IServiceManager
{
    IAuthorizationEmailService EmailService { get; }
    IAuthorizationService AuthorizationService { get; }
    IWishlistService WishlistService { get; }
    IPhotoService PhotoService { get; }

}
