using Domain.Entities;
using Domain.Models;
using MediatR;

namespace Application.MediatR.Queries;

public record GetBooksQuery(int page):IRequest<PagedList<BookDto>>;

public record GetBooksByCategoryQuery(int categoryId, int page):IRequest<PagedList<BookDto>>;

public record GetBooksByNameQuery(string bookname, int page) : IRequest<PagedList<BookDto>> 
{
    public string Name { get; set; } = bookname.Trim().ToLower();
}
