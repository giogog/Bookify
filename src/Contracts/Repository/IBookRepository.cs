using Domain.Entities;

namespace Contracts;

public interface IBookRepository
{
    Task CreateBookAsync(Book book);
    void DeleteBook(Book book);
    void DeletePhoto(Photo photo);
    void UpdateBook(Book book);
    Task<Book?> GetBookByIdAsync(Guid id);
    Task<Book?> GetBookByNameAsync(string name);
    IQueryable<Book> Books();
}
