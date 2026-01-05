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
    private readonly int _pageSize = Int32.Parse(configuration["ApiSettings:PageSize"]);

    public async Task<PagedList<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting books by page");
        var bookDtoquery = _repositoryManager.BookRepository.Books()
            .Include(b => b.Ratings)
            .OrderBy(b => b.Price)
            .Select(b => new BookDto(b.Name, b.Price, b.Rating, b.Author.Name, b.Author.Surname, b.Category.Name, b.SalePrice, b.Sale,b.Photo.Url))
            .AsSplitQuery();

        return await PagedList<BookDto>.CreateAsync(bookDtoquery, request.page, _pageSize);

    }
}
