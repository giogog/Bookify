using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure;

public static class DataSeeder
{
    public static Guid adminRoleGuid = Guid.NewGuid();
    public static Guid userRoleGuid = Guid.NewGuid();
    public static Guid adminGuid = Guid.NewGuid();
    public static Guid userGuid = Guid.NewGuid();

    public static void SeedUsers(this ModelBuilder modelBuilder)
    {

        
        PasswordHasher<User> hasher = new();

        var adminUser = new User
        {
            Id = adminGuid,
            UserName = "admin",
            NormalizedUserName = "ADMIN@GMAIL.COM",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "555334455",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0,
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123");

        var regularUser = new User
        {
            Id = userGuid,
            UserName = "User1",
            NormalizedUserName = "USER1",
            Email = "user@gmail.com",
            NormalizedEmail = "USER@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "555334457",
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0,
        };
        regularUser.PasswordHash = hasher.HashPassword(regularUser, "Ab123123");

        modelBuilder.Entity<User>().HasData(adminUser, regularUser);
    }

    public static void SeedRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleGuid, Name = "Admin", NormalizedName = "ADMIN" },
            new Role { Id = userRoleGuid, Name = "User", NormalizedName = "USER" }
        );
    }

    public static void SeedUserRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { RoleId = adminRoleGuid, UserId = adminGuid },
            new UserRole { RoleId = userRoleGuid, UserId = userGuid }
        );
    }

}