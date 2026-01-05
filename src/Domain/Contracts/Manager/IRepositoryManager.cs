namespace Contracts;

public interface IRepositoryManager
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IBookRepository BookRepository { get; }
    IAuthorRepository AuthorRepository { get; }
    IRatingRepository RatingRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IWishlistRepository WishlistRepository { get; }
    
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task SaveAsync();
}
