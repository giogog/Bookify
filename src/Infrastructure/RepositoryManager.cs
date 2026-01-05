using Contracts;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure;

public class RepositoryManager : IRepositoryManager
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;
    private readonly Lazy<IBookRepository> _bookRepository;
    private readonly Lazy<IAuthorRepository> _authorRepository;
    private readonly Lazy<ICategoryRepository> _categoryRepository;
    private readonly Lazy<IRatingRepository> _ratingRepository; 
    private readonly Lazy<IWishlistRepository> _wishlistRepository;
    public RepositoryManager(ApplicationDbContext context, UserManager<User> userManager,RoleManager<Role> roleManager)
    {
        _context = context;
        _userRepository = new(() => new UserRepository(userManager));
        _roleRepository = new (()=> new RoleRepository(roleManager));
        _bookRepository = new(() => new BookRepository(context));
        _authorRepository = new(() => new AuthorRepository(context));
        _categoryRepository = new (()=> new CategoryRepository(context));
        _ratingRepository = new (()=> new RatingRepository(context));
        _wishlistRepository = new (()=> new WishlistRepository(context));
    }


    public RepositoryManager(ApplicationDbContext context)
    {
        _context = context;
        _bookRepository = new(() => new BookRepository(context));
        _authorRepository = new(() => new AuthorRepository(context));
        _categoryRepository = new(() => new CategoryRepository(context));
        _wishlistRepository = new(() => new WishlistRepository(context));
    }
    public IUserRepository UserRepository => _userRepository.Value;
    public IRoleRepository RoleRepository => _roleRepository.Value;
    public ICategoryRepository CategoryRepository => _categoryRepository.Value;
    public IBookRepository BookRepository => _bookRepository.Value; 
    public IAuthorRepository AuthorRepository => _authorRepository.Value;  
    public IRatingRepository RatingRepository => _ratingRepository.Value;  
    public IWishlistRepository WishlistRepository => _wishlistRepository.Value;

    public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
            await _transaction.CommitAsync();
    }
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }
    public void Dispose() => _transaction?.Dispose();
    public Task SaveAsync() => _context.SaveChangesAsync();
}
