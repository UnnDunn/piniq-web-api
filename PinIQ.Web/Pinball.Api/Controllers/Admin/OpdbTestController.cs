using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pinball.Api.Services.Interfaces;
using Pinball.OpdbClient.Entities;
using System;
using System.Threading.Tasks;

namespace Pinball.Api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class OpdbTestController : ControllerBase
    {
        private ITestOpdbService _opdbService;
        private ILogger<OpdbTestController> _logger;

        public OpdbTestController(ITestOpdbService opdbService, ILogger<OpdbTestController> logger)
        {
            _opdbService = opdbService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> GetAsync(string id)
        {
            var opdbId = (OpdbId?)id;
            if (opdbId == null) throw new ArgumentException(nameof(id));
            var result = await _opdbService.GetAsync(opdbId);
            return result;
        } 
    }
}