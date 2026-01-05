using Domain.Entities;

namespace Contracts;

public interface IRatingRepository
{
    Task CreateRatingAsync(Rating rating);
    void DeleteRating(Rating rating);
    void UpdateRating(Rating rating);
    IQueryable<Rating> Ratings();
}
