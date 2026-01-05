using Domain.Entities;

namespace Contracts;

public interface IAuthorRepository
{
    Task CreateAuthorAsync(Author author);
    void DeleteAuthor(Author author);
    void UpdateAuthor(Author author);
    Task<Author?> GetAuthorByIdAsync(Guid id);
    Task<Author?> GetAuthorByNameAsync(string name, string surname);
    IQueryable<Author> Authors();
}
