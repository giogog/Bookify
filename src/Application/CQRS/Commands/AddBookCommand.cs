using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.MediatR.Commands;

public record AddBookCommand(string bookname, decimal Price, string authorName, string authorSurname, string bookcategory) : IRequest 
{
    public string Name => bookname.Trim();
    public string AuthName => authorName.Trim();
    public string AuthSurname => authorSurname.Trim();
    public string Category => bookcategory.Trim();
}



