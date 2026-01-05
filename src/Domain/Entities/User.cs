using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public ICollection<UserRole>? Roles { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; }
}
