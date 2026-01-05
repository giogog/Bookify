using Application.MediatR.Queries;
using Contracts;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Query;

public class GetBooksByCategoryQueryHandler(IRepositoryManager repositoryManager, ILogger<GetBooksByCategoryQueryHandler> logger, IConfiguration configuration) :IRequestHandler<GetBooksByCategoryQuery,PagedList<BookDto>>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<GetBooksByCategoryQueryHandler> _logger = logger;
    private readonly int _pageSize = Int32.Parse(configuration["ApiSettings:PageSize"]);

    public async Task<PagedList<BookDto>> Handle(GetBooksByCategoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting books by category");
        bool CategoryExists = await _repositoryManager.CategoryRepository.Categories().AnyAsync(c => c.Id == request.categoryId);

        if (!CategoryExists) 
        {
            _logger.LogError($"Category on this Id: {request.categoryId} doesn't exists");
            throw new NotFoundException($"Category on this Id: {request.categoryId} doesn't exists");
        }

        var query = _repositoryManager.BookRepository.Books()
            .Where(b => b.CategoryId == request.categoryId)
            .Include(b=>b.Ratings)
            .OrderBy(b => b.Price)
            .Select(b => new BookDto(b.Name, b.Price,b.Rating, b.Author.Name, b.Author.Surname, b.Category.Name, b.SalePrice, b.Sale, b.Photo.Url))
            .AsSplitQuery();

        return await PagedList<BookDto>.CreateAsync(query, request.page, _pageSize);
    }
}
