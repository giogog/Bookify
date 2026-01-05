using Domain.Entities;
using MediatR;

namespace Application.MediatR.Queries;

public record GetCategoriesQuery():IRequest<IEnumerable<string>>;
