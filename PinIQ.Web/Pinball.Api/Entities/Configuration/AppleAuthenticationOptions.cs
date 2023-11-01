namespace Pinball.Api.Entities.Configuration;

public record AppleAuthenticationOptions(string ClientId, string TeamId, string? KeyId, AppleCertificateType Type = AppleCertificateType.Local);

public enum AppleCertificateType { Local, AzureKeyVault };