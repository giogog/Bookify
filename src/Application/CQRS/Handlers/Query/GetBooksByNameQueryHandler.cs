using Application.MediatR.Queries;
using Contracts;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Query;

public class GetBooksByNameQueryHandler(IRepositoryManager repositoryManager, ILogger<GetBooksByNameQueryHandler> logger, IConfiguration configuration) : IRequestHandler<GetBooksByNameQuery, PagedList<BookDto>>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<GetBooksByNameQueryHandler> _logger = logger;
    private readonly int _pageSize = Int32.Parse(configuration["ApiSettings:PageSize"]);

    public async Task<PagedList<BookDto>> Handle(GetBooksByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting books by name");
        var query = _repositoryManager.BookRepository.Books()
            .Where(b=>b.Name.ToLower().Contains(request.Name))
            .Include(b => b.Ratings)
            .OrderBy(b => b.Price)
            .Select(b => new BookDto(b.Name, b.Price, b.Rating, b.Author.Name, b.Author.Surname, b.Category.Name, b.SalePrice, b.Sale, b.Photo.Url))
            .AsSplitQuery();

        return await PagedList<BookDto>.CreateAsync(query, request.page, _pageSize);
    }
}
