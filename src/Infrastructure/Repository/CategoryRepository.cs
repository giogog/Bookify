using Contracts;
using Domain.Entities;
using Infrastructure.Context;

namespace Infrastructure.Repository;

public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context),ICategoryRepository 
{
    public IQueryable<Category> Categories() => FindAll();

    public async Task CreateCategoryAsync(Category category) => await Create(category);

    public void DeleteCategory(Category category) => Delete(category);

    public void UpdateCategory(Category category) => Update(category);  
}
