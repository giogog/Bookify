using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Book
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
    public required decimal Price { get; set; }

    [Range(1, 100, ErrorMessage = "Sale Ammount must be between 1 and 100")]
    public int Sale { get; set; } = 0;
    public double SalePrice => Sale > 0 ? (double)Price * (100-Sale)/100 : (double)Price;
    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }
    public double Rating => Ratings != null && Ratings.Count > 0
        ? Math.Round(Ratings.Sum(r => r.Stars) / (double)Ratings.Count, 1)
        : 0.0;
    public ICollection<Rating>? Ratings { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; }
    public Photo? Photo { get; set; }
}
