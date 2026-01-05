namespace Domain.Entities;

public class Wishlist
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid BookId { get; set; }
    public Book? Book { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
