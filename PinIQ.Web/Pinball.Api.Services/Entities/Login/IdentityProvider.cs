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