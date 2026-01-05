using Contracts;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class BookRepository(ApplicationDbContext context) : BaseRepository<Book>(context), IBookRepository
    {
        public IQueryable<Book> Books() => FindAll();

        public async Task CreateBookAsync(Book book) => await Create(book);

        public void DeleteBook(Book book) => Delete(book);

        public void DeletePhoto(Photo photo) => _context.Photos.Remove(photo);

        public void UpdateBook(Book book) => Update(book);

        public async Task<Book?> GetBookByIdAsync(Guid id) => await _context.Set<Book>().FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Book?> GetBookByNameAsync(string name) => await _context.Set<Book>().FirstOrDefaultAsync(b => b.Name == name);

        
    }
}
