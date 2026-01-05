using Contracts;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class AuthorRepository(ApplicationDbContext context) : BaseRepository<Author>(context), IAuthorRepository
{
    public IQueryable<Author> Authors() => FindAll();

    public async Task CreateAuthorAsync(Author author) => await Create(author);

    public void DeleteAuthor(Author author) => Delete(author);

    public async Task<Author?> GetAuthorByIdAsync(Guid id) => await _context.Set<Author>().FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Author?> GetAuthorByNameAsync(string name,string surname) => await _context.Set<Author>().FirstOrDefaultAsync(a => a.Name == name && a.Surname == surname);

    public  void UpdateAuthor(Author author) => Update(author);
}
