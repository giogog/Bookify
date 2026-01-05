using Application.Extensions;
using MediatR;

namespace Application.MediatR.Commands;

public record AddRatingCommand(string uid, string bid, int stars) : IRequest
{
    public Guid UserId => uid.GetGuid();
    public Guid BookId => bid.GetGuid();
    public int Star => stars.IntOverLimits(1, 100) ? stars : throw new ArgumentException("Stars must be from 1 to 5");

}
