using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinball.Api.Services.Interfaces;
using Pinball.Entities.Opdb;

namespace Pinball.Api.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
[AllowAnonymous]
public class OpdbTestController : ControllerBase
{
    private readonly ITestOpdbService _opdbService;

    public OpdbTestController(ITestOpdbService opdbService)
    {
        _opdbService = opdbService;
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