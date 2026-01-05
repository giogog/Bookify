using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Context;


public class ApplicationDbContext : IdentityDbContext<User, Role, Guid
    , IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>
    , IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Rating> Ratings => Set<Rating>();


    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.SeedRoles();
        builder.SeedUsers();
        builder.SeedUserRoles();

        builder.ApplyConfigurationsFromAssembly(
            typeof(Configuration.UserConfiguration).Assembly
        );


    }
}
