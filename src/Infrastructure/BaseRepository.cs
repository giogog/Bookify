using Contracts;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure;

public abstract class BaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual async Task Create(T entity) => await _context.Set<T>().AddAsync(entity);

    public virtual void Delete(T entity) => _context.Set<T>().Remove(entity);

    public virtual void Update(T entity) => _context.Set<T>().Update(entity);

    public virtual IQueryable<T> FindAll() => _context.Set<T>();
}
