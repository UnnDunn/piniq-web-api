using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pinball.Api.Services.Interfaces;

namespace Pinball.Api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class PinballMachinesController : ControllerBase
    {
        private readonly IPinballMachineCatalogService _catalogService;

        public PinballMachinesController(IPinballMachineCatalogService catalogService)
        {
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