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
        if (!source.Expression.ToString().Contains("OrderBy"))
        {
            throw new InvalidOperationException("The query must contain an 'OrderBy' clause before using 'Skip' and 'Take'.");
        }

        var count = await source.CountAsync();
        var items  = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync();
        
        if (!items.Any())
        {
            throw new NotFoundException("Items Not Found");
        }
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

}