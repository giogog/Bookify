using Application.Extensions;
using MediatR;

namespace Application.MediatR.Commands;

public record DeleteBookCommand(string bid):IRequest
{
    public Guid BookId => bid.GetGuid();
}
