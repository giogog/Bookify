using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Rating
{
    public int Id { get; set; }
    [Range(1, 5, ErrorMessage = "Stars must be between 1 and 5.")]
    public required int Stars { get; set; }
    public required Guid BookId { get; set; }
    public Book? Book { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }

}
