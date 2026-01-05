using Application.Extensions;
using MediatR;

namespace Application.CQRS.Commands;

public record RemovePhotoFromBookCommand(string bid) : IRequest
{
    public Guid BookId = bid.GetGuid();
}