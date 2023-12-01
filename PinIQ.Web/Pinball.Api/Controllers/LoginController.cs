using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Apple;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Pinball.Api.Entities.Configuration;
using Pinball.Api.Entities.Responses;
using Pinball.Api.Services.Entities.Configuration;
using Pinball.Api.Services.Entities.Login;
using Pinball.Api.Services.Interfaces.Impl;

namespace Pinball.Api.Controllers;

[ApiController, Route("[controller]"), AllowAnonymous]
public partial class LoginController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly DeveloperOptions _developerOptions;
    private readonly MyJwtBearerOptions _myJwtOptions;
    private readonly ILogger<LoginController> _logger;

    private const string CallbackScheme = "piniq";

    public LoginController(LoginService loginService, IOptions<DeveloperOptions> developerOptions, IOptions<MyJwtBearerOptions> myJwtOptions, ILogger<LoginController> logger)
    {
        _loginService = loginService;
        _logger = logger;
        _myJwtOptions = myJwtOptions.Value;
        _developerOptions = developerOptions.Value;
    }

    [HttpGet("{scheme}"), AllowAnonymous]
    public async Task Login([FromRoute] string scheme)
    {
        var schemes = new List<string> { 
            AppleAuthenticationDefaults.AuthenticationScheme,
            CookieAuthenticationDefaults.AuthenticationScheme, 
            GoogleDefaults.AuthenticationScheme, 
            JwtBearerDefaults.AuthenticationScheme 
        };

        var canonicalScheme =
            schemes.FirstOrDefault(s => scheme.Equals(s, StringComparison.InvariantCultureIgnoreCase));

        if (canonicalScheme is null) throw new Exception("Invalid authentication scheme");

        LogCanonicalScheme(canonicalScheme);

        var auth = await Request.HttpContext.AuthenticateAsync(canonicalScheme);

        LogAuthResult(auth.Succeeded, auth.Properties?.Items);
        
        if (!auth.Succeeded
            || auth?.Principal is null
            || !auth.Principal.Identities.Any(id => id.IsAuthenticated))
        {
            await Request.HttpContext.ChallengeAsync(canonicalScheme);
        }
        else
        {
            var provider = canonicalScheme switch
            {
                AppleAuthenticationDefaults.AuthenticationScheme => IdentityProvider.Apple,
                GoogleDefaults.AuthenticationScheme => IdentityProvider.Google,
                _ => IdentityProvider.Self
            };

            var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var claimsDictionary = claims?.Select(c => new KeyValuePair<string, string>(c.Type, c.Value));
                LogRetrievedClaims(claimsDictionary);
            }
            var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            if (email is not null)
            {
                var providerId = new ProviderIdentity(email, provider);
                var accessToken = _loginService.BuildIdToken(providerId);
                var refreshTokenResult = _loginService.BuildRefreshToken(providerId, DateTime.UtcNow);

                var qs = new Dictionary<string, string>()
                {
                    { "access_token", accessToken.AccessToken ?? string.Empty },
                    { "refresh_token", refreshTokenResult.RefreshToken ?? string.Empty },
                    { "expires_in", (accessToken.Expiry?.ToUnixTimeSeconds() ?? -1).ToString() },
                    { "email", email }
                };


                var qString = string.Join("&", qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                var returnUrlBuilder = new UriBuilder(CallbackScheme, string.Empty)
                {
                    Fragment = qString
                };

                var returnUrl = returnUrlBuilder.ToString();
                LogReturnUrl(returnUrl);
                Request.HttpContext.Response.Redirect(returnUrlBuilder.ToString());
            }
            else
            {
                await Request.HttpContext.ForbidAsync(canonicalScheme);
            }
        }
    }

    [LoggerMessage(EventId = 1304, Level = LogLevel.Debug, Message = "Redirecting to {returnUrl}")]
    private partial void LogReturnUrl(string returnUrl);

    [LoggerMessage(EventId = 1303, Level = LogLevel.Debug, Message = "Login Success: {authSucceeded}, Properties: {propertiesItems}")]
    private partial void LogAuthResult(bool authSucceeded, IDictionary<string, string?>? propertiesItems);

    [LoggerMessage(EventId = 1302, Level = LogLevel.Debug, Message = "Attempting login using scheme {canonicalScheme}")]
    private partial void LogCanonicalScheme(string canonicalScheme);

    [LoggerMessage(EventId = 1301, Level = LogLevel.Debug, Message = "Claims retrieved: {claimsDictionary}")]
    private partial void LogRetrievedClaims(IEnumerable<KeyValuePair<string, string>>? claimsDictionary);

    [HttpGet("refresh"), AllowAnonymous]
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
        var accessToken = _loginService.BuildIdToken(providerId).AccessToken ?? string.Empty;

        string? refreshToken = null;
        if (claims.TryGetValue(JwtRegisteredClaimNames.Exp, out var expirationDateUnix))
        {
            var expirationDateOffset = DateTimeOffset.FromUnixTimeSeconds((long)expirationDateUnix);
            var minExpirationDate = new DateTimeOffset(DateTime.UtcNow.AddDays(_myJwtOptions.RefreshTokenRenewalWindowDays));

            if (expirationDateOffset < minExpirationDate)
            {
                var originalLoginDate =
                    claims.TryGetValue(LoginService.OriginalLoginDate, out var old)
                        ? DateTime.Parse(old.ToString()!)
                        : DateTime.UtcNow;

                refreshToken = _loginService.BuildRefreshToken(providerId, originalLoginDate).RefreshToken;
            }
        }

        var result = new LoginTokenResponse(accessToken, refreshToken);

        return Ok(result);
    }

    [HttpGet("testtoken"), ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
    public IActionResult TestToken([FromQuery] string testKey)
    {
        if (_developerOptions.LoginTestKey is null || _developerOptions.LoginTestKey != testKey)
        {
            return Forbid();
        }

        var testId = new ProviderIdentity("TestID", IdentityProvider.Self);

        var originalLoginDate = DateTime.UtcNow;
        var accessToken = _loginService.BuildIdToken(testId).AccessToken ?? string.Empty;
        var refreshToken = _loginService.BuildRefreshToken(testId, originalLoginDate).RefreshToken;

        var result = new LoginTokenResponse(accessToken, refreshToken);

        return Ok(result);
    }

    [HttpGet("/throwException"), ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult ThrowException()
    {
        throw new Exception($"Exception thrown at {DateTime.UtcNow:g}");
    }
}