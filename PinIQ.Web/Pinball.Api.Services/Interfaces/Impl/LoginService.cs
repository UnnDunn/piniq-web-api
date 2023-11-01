using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pinball.Api.Services.Entities.Configuration;
using Pinball.Api.Services.Entities.Login;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Pinball.Api.Services.Interfaces.Impl;

public partial class LoginService
{
    private readonly ILogger<LoginService> _logger;
    private readonly MyJwtBearerOptions _myJwtOptions;

    public const string OriginalIssuer = "ogiss";
    public const string OriginalIdentifier = "ogid";
    public const string OriginalLoginDate = "ogld";
    
    public LoginService(ILogger<LoginService> logger, IOptions<MyJwtBearerOptions> jwtOptions)
    {
        _logger = logger;
        _myJwtOptions = jwtOptions.Value;
    }

    public string BuildIdToken(ProviderIdentity identity, IEnumerable<Claim>? additionalClaims = null)
    {
        LogActionGeneratingIdToken(identity);
        var claims = new List<Claim>
        {
            new Claim(OriginalIdentifier, identity.Identifier),
            new Claim(OriginalIssuer, identity.Provider.ToString())
        };

        if (additionalClaims is not null)
        {
            claims.AddRange(additionalClaims);
        }
        
        // add boilerplate claims
        claims.AddRange(
            _myJwtOptions
                .ValidAudiences
                .Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

        var timeStamp = DateTime.UtcNow;

        var idTokenSigningKey = _myJwtOptions.SigningKeys.Single(s => s.Issuer == _myJwtOptions.ValidIssuer);
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(idTokenSigningKey.Value));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: _myJwtOptions.ValidIssuer,
            claims: claims, expires: DateTime.Now.AddMinutes(_myJwtOptions.AccessTokenExpirationMinutes), signingCredentials: signInCredentials, notBefore: timeStamp);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    public string BuildRefreshToken(ProviderIdentity identity, DateTime originalLoginDate,
        IEnumerable<Claim>? additionalClaims = null)
    {
        LogActionGeneratingRefreshToken(identity, originalLoginDate);
        var claims = new List<Claim>
        {
            new Claim(OriginalIdentifier, identity.Identifier),
            new Claim(OriginalIssuer, identity.Provider.ToString()),
            new Claim(OriginalLoginDate, originalLoginDate.ToString("u"))
        };

        if (additionalClaims is not null)
        {
            claims.AddRange(additionalClaims);
        }

        // add boilerplate claims
        claims.AddRange(
            _myJwtOptions
                .ValidAudiences
                .Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

        var timeStamp = DateTime.UtcNow;
        var tokenSigningKey = _myJwtOptions.SigningKeys.Single(s => s.Issuer == _myJwtOptions.ValidIssuer);
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSigningKey.Value));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: _myJwtOptions.ValidIssuer + "_refresh",
            claims: claims, expires: DateTime.Now.AddDays(_myJwtOptions.RefreshTokenExpirationDays), signingCredentials: signInCredentials, notBefore: timeStamp);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    public async Task<IDictionary<string, object>> ReadRefreshToken(string refreshToken)
    {
        var tokenSigningKey = _myJwtOptions.SigningKeys.Single(s => s.Issuer == _myJwtOptions.ValidIssuer);
        var validationOptions = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _myJwtOptions.ValidIssuer + "_refresh",
            ValidAudiences = _myJwtOptions.ValidAudiences,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSigningKey.Value))
        };
        var jwtHandler = new JwtSecurityTokenHandler();
        var tokenValidationResult = await jwtHandler.ValidateTokenAsync(refreshToken, validationOptions);

        if (!tokenValidationResult.IsValid)
        {
            LogExceptionRefreshTokenValidation(tokenValidationResult.Exception);
            throw tokenValidationResult.Exception;
        }
        
        var claimCount = tokenValidationResult.Claims.Count;
        LogSuccessRefreshTokenValidation(claimCount);
        return tokenValidationResult.Claims;
    }
    
    #region Logging

    // Methods for writing source-generated logging statements
    // All logging statements in this service must have event IDs "12xx"

    [LoggerMessage(EventId = 1201, Level = LogLevel.Information, Message = "Generating ID token for supplied {ProviderIdentity}")]
    public partial void LogActionGeneratingIdToken(ProviderIdentity providerIdentity);

    [LoggerMessage(EventId = 1202, Level = LogLevel.Information, Message = "Generating refresh token for supplied {ProviderIdentity} with original login date {originalLoginDate}")]
    public partial void LogActionGeneratingRefreshToken(ProviderIdentity providerIdentity, DateTime originalLoginDate);

    [LoggerMessage(EventId = 1203, Level = LogLevel.Information, Message = "Refresh token validated successfully; {claimCount} claims found")]
    public partial void LogSuccessRefreshTokenValidation(int claimCount);

    [LoggerMessage(EventId = 1204, Level = LogLevel.Error, Message = "Refresh token validation failed")]
    public partial void LogExceptionRefreshTokenValidation(Exception ex);

    #endregion
}