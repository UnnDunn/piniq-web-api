using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pinball.Api.Entities.Responses;
using Pinball.Api.Services.Entities;
using Pinball.Api.Services.Entities.Exceptions;
using Pinball.Api.Services.Interfaces;

namespace Pinball.Api.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class PinballMachineCatalogSnapshotsController : ControllerBase
    {
        private IPinballMachineCatalogService _catalogService;
        private ILogger<PinballMachineCatalogSnapshotsController> _logger;

        public PinballMachineCatalogSnapshotsController(IPinballMachineCatalogService catalogService, ILogger<PinballMachineCatalogSnapshotsController> logger)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        [HttpGet]
        [Route("")] //GET /api/admin/PinballMachineCatalogSnapshots
        public async Task<ActionResult<IEnumerable<CatalogSnapshotDigest>>> GetAll()
        {
            var snapshots = await _catalogService.GetAllCatalogSnapshotsAsync();

            var digestTasks = await Task.WhenAll(snapshots.Select(s => CatalogSnapshotDigest.FromSnapshot(s)));
            var result = digestTasks.ToList();

            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")] //POST /api/admin/PinballMachineCatalogSnapshots/Import
        public async Task<ActionResult<CatalogSnapshotDigest>> Import()
        {
            try
            {
                var snapshot = await _catalogService.ImportNewCatalogSnapshotAsync();
                var result = CatalogSnapshotDigest.FromSnapshot(snapshot);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            } catch (OpdbException o)
            {
                return BadRequest(o.Message);
            }
        }

        [HttpGet]
        [Route("{id}")] //GET /api/admin/PinballMachineCatalogSnapshots/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogSnapshot>> Get(int id)
        {
            var result = await _catalogService.GetCatalogSnapshotAsync(id);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("[action]")] //POST /api/admin/PinballMachineCatalogSnapshots/PublishLatest
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CatalogSnapshotPublishResult>> PublishLatest()
        {
            try
            {
                var result = await _catalogService.PublishCatalogSnapshotAsync();
                return Ok(result);
            }
            catch (CatalogSnapshotException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("currentSnapshot")] //GET /api/admin/PinballMachineCatalogSnapshots/currentSnapshot
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogSnapshot>> GetPublished()
        {
            var result = await _catalogService.GetPublishedCatalogSnapshotAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")] //DELETE /api/admin/PinballMachineCatalogSnapshots/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _catalogService.DeleteCatalogSnapshotAsync(id);
                return Ok();
            } catch (KeyNotFoundException)
            {
                return NotFound();
            } catch (CatalogSnapshotException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}