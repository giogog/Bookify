using Domain.Entities;

namespace Contracts;

public interface ICategoryRepository
{
    Task CreateCategoryAsync(Category category);
    void DeleteCategory(Category category);
    void UpdateCategory(Category category);
    IQueryable<Category> Categories();
}
