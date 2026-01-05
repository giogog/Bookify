using Application.Extensions;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.MediatR.Commands;

public record AddBookSaleCommand(string bid, int salePercentage):IRequest
{
    public Guid BookId => bid.GetGuid();
    public int Percentage => salePercentage.IntOverLimits(1,100)?salePercentage:throw new ArgumentException("Incorrect Percentage");

}
