using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pinball.Api.Controllers;

[ApiController]
[AllowAnonymous]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        _logger.LogError(exceptionHandlerFeature?.Error, "Error occurred handling request to path {path}",
            exceptionHandlerFeature?.Path);

        return Problem();
    }
}