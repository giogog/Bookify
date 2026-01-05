using Application.Extensions;
using MediatR;

namespace Application.MediatR.Commands
{
    public record UpdateBookCommand(string bid, string bookname, decimal bookprice):IRequest
    {
        public Guid BookId => bid.GetGuid();
        public string Name => bookname.Trim();
        public decimal Price => bookprice;
    }
}
