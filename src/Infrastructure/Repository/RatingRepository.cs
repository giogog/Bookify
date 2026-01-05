using Contracts;
using Domain.Entities;
using Infrastructure.Context;

namespace Infrastructure.Repository;

public class RatingRepository(ApplicationDbContext context) : BaseRepository<Rating>(context), IRatingRepository
{
    public IQueryable<Rating> Ratings() => FindAll();

    public async Task CreateRatingAsync(Rating rating) => await Create(rating);

    public void DeleteRating(Rating rating) => Delete(rating);

    public void UpdateRating(Rating rating) => Update(rating);
}
