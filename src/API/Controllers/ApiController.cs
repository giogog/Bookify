using Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    protected readonly IServiceManager _serviceManager;
    protected readonly IMediator _mediator;
    protected ApiResponse _response;

    public ApiController(IServiceManager serviceManager) => _serviceManager = serviceManager;
    public ApiController(IMediator mediator) => _mediator = mediator;
    public ApiController(IServiceManager serviceManager,IMediator mediator)
    {
        _serviceManager = serviceManager;
        _mediator = mediator;
    }
}
