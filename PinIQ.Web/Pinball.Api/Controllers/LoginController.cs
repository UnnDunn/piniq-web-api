using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Apple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Pinball.Api.Entities.Configuration;
using Pinball.Api.Entities.Responses;
using Pinball.Api.Services.Entities.Configuration;
using Pinball.Api.Services.Entities.Login;
using Pinball.Api.Services.Interfaces.Impl;

namespace Pinball.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly DeveloperOptions _developerOptions;
    private readonly MyJwtBearerOptions _myJwtOptions;

    public LoginController(LoginService loginService, IOptions<DeveloperOptions> developerOptions, IOptions<MyJwtBearerOptions> myJwtOptions)
    {
        _loginService = loginService;
        _myJwtOptions = myJwtOptions.Value;
        _developerOptions = developerOptions.Value;
    }

    [Authorize(AuthenticationSchemes = AppleAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet]
    public Task<IActionResult> Apple()
    {
        throw new NotImplementedException();
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> Refresh([FromQuery] string token)
    {
        var claims = await _loginService.ReadRefreshToken(token);

        if (!claims.TryGetValue(LoginService.OriginalIdentifier, out var originalIdentifier)
            || !claims.TryGetValue(LoginService.OriginalIssuer, out var originalIssuer))
        {
            return BadRequest();
        }

        if (!Enum.TryParse<IdentityProvider>(originalIssuer.ToString(), out var originalIssuerEnum))
            return BadRequest();
        
        var providerId = new ProviderIdentity(originalIdentifier.ToString()!, originalIssuerEnum);
        var accessToken = _loginService.BuildIdToken(providerId);

        string? refreshToken = null;
        if (claims.TryGetValue(JwtRegisteredClaimNames.Exp, out var expirationDateUnix))
        {
            var expirationDateOffset = DateTimeOffset.FromUnixTimeSeconds((int)expirationDateUnix);
            var minExpirationDate = new DateTimeOffset(DateTime.UtcNow.AddDays(_myJwtOptions.RefreshTokenRenewalWindowDays));

            if (expirationDateOffset < minExpirationDate)
            {
                var originalLoginDate =
                    claims.TryGetValue(LoginService.OriginalLoginDate, out var old)
                        ? DateTime.Parse(old.ToString()!)
                        : DateTime.UtcNow;

                refreshToken = _loginService.BuildRefreshToken(providerId, originalLoginDate);
            }
        }

        var result = new LoginTokenResponse(accessToken, refreshToken);

        return Ok(result);
    }

    [HttpGet, AllowAnonymous]
    public IActionResult TestToken([FromQuery] string testKey)
    {
        if (_developerOptions.LoginTestKey is null || _developerOptions.LoginTestKey != testKey)
        {
            return Forbid();
        }

        var testId = new ProviderIdentity("TestID", IdentityProvider.Self);

        var originalLoginDate = DateTime.UtcNow;
        var accessToken = _loginService.BuildIdToken(testId);
        var refreshToken = _loginService.BuildRefreshToken(testId, originalLoginDate);

        var result = new LoginTokenResponse(accessToken, refreshToken);

        return Ok(result);
    }
}