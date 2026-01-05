using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b=>b.Id);
        builder.Property(b => b.Price)
        .HasPrecision(18, 2);
        builder.HasOne(b => b.Author)
            .WithMany(a=>a.Books)
            .HasForeignKey(b => b.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Ratings)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(b=>b.Category)
            .WithMany(c=>c.Books)
            .HasForeignKey(b=>b.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Wishlists)
            .WithOne(w => w.Book)
            .HasForeignKey(w => w.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Photo)
            .WithOne()
            .HasForeignKey<Photo>(p=>p.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
