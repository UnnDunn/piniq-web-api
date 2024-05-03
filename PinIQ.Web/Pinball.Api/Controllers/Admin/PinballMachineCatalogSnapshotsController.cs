using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pinball.Api.Services.Entities.Exceptions;
using Pinball.Api.Services.Interfaces;
using Pinball.Entities.Api.Responses.PinballCatalog;
using CatalogSnapshotPublishResult = Pinball.Entities.Api.Responses.PinballCatalog.CatalogSnapshotPublishResult;

namespace Pinball.Api.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public partial class PinballMachineCatalogSnapshotsController : ControllerBase
{
    private readonly IPinballMachineCatalogService _catalogService;
    private readonly ILogger<PinballMachineCatalogSnapshotsController> _logger;

    public PinballMachineCatalogSnapshotsController(IPinballMachineCatalogService catalogService,
        ILogger<PinballMachineCatalogSnapshotsController> logger)
    {
        _logger = logger;
        _catalogService = catalogService;
    }

    [HttpGet]
    [Route("")] //GET /api/admin/PinballMachineCatalogSnapshots
    public async Task<ActionResult<IEnumerable<CatalogSnapshot>>> GetAll()
    {
        var snapshots = await _catalogService.GetAllCatalogSnapshotsAsync();

        return Ok(snapshots);
    }

    [HttpPost]
    [Route("Import")] //POST /api/admin/PinballMachineCatalogSnapshots/Import
    public async Task<ActionResult<CatalogSnapshot>> Import()
    {
        try
        {
            var snapshot = await _catalogService.ImportNewCatalogSnapshotAsync();
            return CreatedAtAction(nameof(Get), new { id = snapshot.Id }, snapshot);
        }
        catch (OpdbException o)
        {
            return BadRequest(o.Message);
        }
    }

    [HttpGet]
    [Route("{id:int}")] //GET /api/admin/PinballMachineCatalogSnapshots/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CatalogSnapshot>> Get(int id)
    {
        var result = await _catalogService.GetCatalogSnapshotAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [Route(@"{ids:regex(^(\d{{1,3}},)*\d{{1,3}}$)}", Order = 1)] //GET /api/admin/PinballMachineCatalogSnapshots/4,5,6
    public async Task<ActionResult<List<CatalogSnapshot>>> Get(string ids)
    {
        var idArray = ids.Split(',').Select(int.Parse).ToArray();
        var result = await _catalogService.GetCatalogSnapshotsAsync(idArray);
        return Ok(result);
    }

    [HttpPost]
    [Route("PublishLatest")] //POST /api/admin/PinballMachineCatalogSnapshots/PublishLatest
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

    [HttpPost("Refresh")] //POST /api/admin/PinballMachineCatalogSnapshots/Refresh
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshCatalogSnapshots()
    {
        try
        {
            await _catalogService.RefreshCatalogSnapshotsAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            LogErrorRefreshingCatalogSnapshots(ex);
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("Refresh/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshCatalogSnapshot(int id)
    {
        try
        {
            await _catalogService.RefreshCatalogSnapshotAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            LogErrorRefreshingCatalogSnapshots(ex);
            return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    [Route("published")] //GET /api/admin/PinballMachineCatalogSnapshots/published
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CatalogSnapshot>> GetPublished()
    {
        var result = await _catalogService.GetPublishedCatalogSnapshotAsync();
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete]
    [Route("{id:int}")] //DELETE /api/admin/PinballMachineCatalogSnapshots/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _catalogService.DeleteCatalogSnapshotAsync(id);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (CatalogSnapshotException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #region Logging

    [LoggerMessage(EventId = 1501, Level = LogLevel.Error,
        Message = "Failed to refresh catalog snapshot information")]
    private partial void LogErrorRefreshingCatalogSnapshots(Exception ex);

    #endregion
}