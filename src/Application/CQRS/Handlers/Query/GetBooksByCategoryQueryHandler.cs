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
    private readonly int _pageSize = int.TryParse(configuration["ApiSettings:PageSize"], out var pageSize) && pageSize > 0
        ? pageSize
        : 10;

    public async Task<PagedList<BookDto>> Handle(GetBooksByCategoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting books by category");
        var categoryExists = await _repositoryManager.CategoryRepository.Categories()
            .AsNoTracking()
            .AnyAsync(c => c.Id == request.categoryId, cancellationToken);

        if (!categoryExists)
        {
            _logger.LogError($"Category on this Id: {request.categoryId} doesn't exists");
            throw new NotFoundException($"Category on this Id: {request.categoryId} doesn't exists");
        }

        var page = request.page < 1 ? 1 : request.page;

        var query = _repositoryManager.BookRepository.Books()
            .AsNoTracking()
            .Where(b => b.CategoryId == request.categoryId)
            .OrderBy(b => b.Price)
            .Select(b => new BookDto(
                b.Name,
                b.Price,
                b.Ratings != null && b.Ratings.Any() ? Math.Round(b.Ratings.Average(r => (double)r.Stars), 1) : 0.0,
                b.Author != null ? b.Author.Name : string.Empty,
                b.Author != null ? b.Author.Surname : null,
                b.Category != null ? b.Category.Name : string.Empty,
                b.SalePrice,
                b.Sale,
                b.Photo != null ? b.Photo.Url : null));

        return await PagedList<BookDto>.CreateAsync(query, page, _pageSize, cancellationToken);
    }
}
