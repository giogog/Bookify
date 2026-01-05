using Application.MediatR.Queries;
using Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.MediatR.Handlers.Query;

public class GetCategoriesQueryHandler(IRepositoryManager repositoryManager, ILogger<GetCategoriesQueryHandler> logger) : IRequestHandler<GetCategoriesQuery, IEnumerable<string>>
{
    private readonly IRepositoryManager _repositoryManager = repositoryManager;
    private readonly ILogger<GetCategoriesQueryHandler> _logger = logger;

    public async Task<IEnumerable<string>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _repositoryManager.CategoryRepository.Categories().Select(c=>c.Name).ToArrayAsync();
    }
}
