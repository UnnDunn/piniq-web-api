using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pinball.Api.Services.Interfaces;
using Pinball.OpdbClient.Entities;

namespace Pinball.Api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class PinballMachinesController : ControllerBase
    {
        private IPinballMachineCatalogService _catalogService;
        private ILogger<PinballMachinesController> _logger;

        public PinballMachinesController(IPinballMachineCatalogService catalogService, ILogger<PinballMachinesController> logger)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<Dictionary<string, int>> Types()
        {
            var result = await _catalogService.GetAllMachineTypesAsync();

            return result;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<Dictionary<string, int>> DisplayTypes()
        {
            var result = await _catalogService.GetAllDisplayTypesAsync();

            return result;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> ResetMachines()
        {
            await _catalogService.ResetCatalogAsync();
            return Ok();
        }
    }
}