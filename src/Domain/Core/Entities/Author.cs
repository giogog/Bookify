using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Author
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters")]
    public required string Surname { get; set; }
    public ICollection<Book>? Books { get; set; }
}
