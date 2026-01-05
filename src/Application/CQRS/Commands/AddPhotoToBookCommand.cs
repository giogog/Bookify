using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.CQRS.Commands;

public record AddPhotoToBookCommand(Guid BookId, IFormFile Photo) : IRequest;


