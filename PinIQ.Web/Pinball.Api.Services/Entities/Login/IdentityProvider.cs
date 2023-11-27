using System;

namespace Pinball.Api.Services.Entities.Login;

public enum IdentityProvider
{
    Self,
    Apple,
    Facebook,
    Google,
    Microsoft
}

public record ProviderIdentity(string Identifier, IdentityProvider Provider);

public record TokenGenerationResult(ProviderIdentity Identity, string? AccessToken = null, string? RefreshToken = null,
    DateTimeOffset? Expiry = null);