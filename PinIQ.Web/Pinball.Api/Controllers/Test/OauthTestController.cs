using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pinball.Api.Controllers.Test;

[Route("api/test/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public partial class OauthTestController : ControllerBase
{
    private readonly ILogger<OauthTestController> _logger;

    public OauthTestController(ILogger<OauthTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("userclaims")]
    public IActionResult GetUserClaimsAsync()
    {
        LogActionGetClaims();
        // enumerate and return claim data
        var claims = HttpContext.User.Claims.Select(c => c)
            .GroupBy(c => c.Type)
            .ToDictionary(c => c.Key, c => c.First().Value);

        return Ok(claims);
    }

    #region Logging

    // Methods for writing source-generated logging statements
    // All logging statements in this service must have event IDs "12xx"

    [LoggerMessage(EventId = 1201, Level = LogLevel.Information, Message = "Getting claims for authenticated user")]
    private partial void LogActionGetClaims();

    #endregion
}