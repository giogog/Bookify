using Application.Exceptions;
using Application.MediatR.Commands;
using Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Command;

public class AddBookCommandHandler(IRepositoryManager repositoryManager,IServiceManager serviceManager, ILogger<AddBookCommandHandler> logger) : IRequestHandler<AddBookCommand>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly IServiceManager _serviceManager = serviceManager;
    private readonly ILogger<AddBookCommandHandler> _logger = logger;

    public async Task Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start handling AddBookCommand for book: {BookName}", request.Name);

        var bookExists = await _repositoryManager.BookRepository.Books()
            .AsNoTracking()
            .AnyAsync(
                b => b.Name == request.Name
                     && b.Author != null
                     && b.Author.Name == request.AuthName
                     && b.Author.Surname == request.AuthSurname,
                cancellationToken);

        // Check if the book already exists
        if (bookExists)
        {
            _logger.LogWarning("Book '{BookName}' by {AuthName} {AuthSurname} already exists.", request.Name, request.AuthName, request.AuthSurname);
            throw new BookAlreadyExistsException($"Book: {request.Name} with this author: {request.AuthName} {request.AuthSurname} already exists.");
        }

        var author = await _repositoryManager.AuthorRepository.GetAuthorByNameAsync(request.AuthName, request.AuthSurname);

        // Start category retrieval (DbContext is NOT thread-safe, so avoid Task.Run)
        var checkCategoryTask = _repositoryManager.CategoryRepository
            .Categories()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == request.Category, cancellationToken);
        // start uploading photo to cloudinary
        
        // Log author status
        _logger.LogInformation(author != null
            ? "Author '{AuthName} {AuthSurname}' found."
            : "Author '{AuthName} {AuthSurname}' does not exist. Creating new author.",
            request.AuthName, request.AuthSurname);

        // Initialize the new book object
        Book book = new Book
        {
            Name = request.Name,
            Price = request.Price,
            Author = author ?? new Author
            {
                Name = request.AuthName,
                Surname = request.AuthSurname
            }
        };

        // Await the category task after main tasks are completed
        var category = await checkCategoryTask;

        // Log category status
        _logger.LogInformation(category != null
            ? "Category '{CategoryName}' found."
            : "Category '{CategoryName}' does not exist. Creating new category.",
            request.Category);

        // Set category information
        book.Category = category ?? new Category { Name = request.Category };
        book.CategoryId = category?.Id ?? 0;

            
        // Try to save the new book
        try
        {
            // Create the book and save changes
            await _repositoryManager.BookRepository.CreateBookAsync(book);
            await _repositoryManager.SaveAsync();

            _logger.LogInformation("Successfully saved book '{BookName}'.", request.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save book '{BookName}'.", request.Name);
            throw; // Re-throw the exception after logging
        }
    }
}
