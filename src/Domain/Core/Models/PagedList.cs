using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        SelectedPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        ItemCount = count;
        PageSize = pageSize;
        AddRange(items);
    }

    public int SelectedPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int ItemCount { get; set; }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        return await CreateAsync(source, pageNumber, pageSize, CancellationToken.None);
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be >= 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be >= 1.");
        }

        if (!source.Expression.ToString().Contains("OrderBy"))
        {
            throw new InvalidOperationException("The query must contain an 'OrderBy' clause before using 'Skip' and 'Take'.");
        }

        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        if (!items.Any())
        {
            throw new NotFoundException("Items Not Found");
        }

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

}