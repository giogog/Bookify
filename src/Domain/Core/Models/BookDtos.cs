namespace Domain.Models;

public record BookDto(string Name, decimal Price,double Rating, string AuthorName, string? AuthorSurname, string Category, double SalePrice, int Sale,string? PhotoUrl);
