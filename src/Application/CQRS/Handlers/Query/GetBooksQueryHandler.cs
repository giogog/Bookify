using Application.MediatR.Queries;
using AutoMapper;
using Contracts;
using Domain.Entities;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Query;

public class GetBooksQueryHandler(IRepositoryManager repositoryManager, ILogger<GetBooksQuery> logger, IConfiguration configuration) : IRequestHandler<GetBooksQuery, PagedList<BookDto>>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<GetBooksQuery> _logger = logger;
    private readonly int _pageSize = int.TryParse(configuration["ApiSettings:PageSize"], out var pageSize) && pageSize > 0
        ? pageSize
        : 10;

    public async Task<PagedList<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting books by page");
        var page = request.page < 1 ? 1 : request.page;

        var bookDtoquery = _repositoryManager.BookRepository.Books()
            .AsNoTracking()
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

        return await PagedList<BookDto>.CreateAsync(bookDtoquery, page, _pageSize, cancellationToken);

    }
}
