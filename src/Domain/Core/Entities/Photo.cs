namespace Domain.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string PublicId { get; set; }
    public Guid BookId { get; set; }
}