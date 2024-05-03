using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pinball.Api.Services.Interfaces;

namespace Pinball.Api.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class PinballMachineCatalogChangelogsController : ControllerBase
{
    private readonly IPinballMachineCatalogService _catalogService;
    private ILogger<PinballMachineCatalogChangelogsController> _logger;

    public PinballMachineCatalogChangelogsController(IPinballMachineCatalogService catalogService,
        ILogger<PinballMachineCatalogChangelogsController> logger)
    {
        _logger = logger;
        _catalogService = catalogService;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<int>> Import()
    {
        var result = await _catalogService.UpdateChangelogsAsync();
        return Ok(result);
    }
}