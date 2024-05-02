using System.Collections.Generic;

namespace Pinball.Api.Services.Entities.Configuration;

public record MyJwtBearerOptions
{
    public MyJwtBearerOptions(IList<string> ValidAudiences, string ValidIssuer,
        IList<MyJwtBearerSigningKeyOptions> SigningKeys)
    {
        this.ValidAudiences = ValidAudiences;
        this.ValidIssuer = ValidIssuer;
        this.SigningKeys = SigningKeys;
    }

    public MyJwtBearerOptions()
    {
    }

    public IList<string> ValidAudiences { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public IList<MyJwtBearerSigningKeyOptions> SigningKeys { get; set; } = null!;

    public int AccessTokenExpirationMinutes { get; set; }

    public int RefreshTokenExpirationDays { get; set; }

    public int RefreshTokenRenewalWindowDays { get; set; }

    public void Deconstruct(out IList<string> validAudiences, out string validIssuer,
        out IList<MyJwtBearerSigningKeyOptions> signingKeys)
    {
        validAudiences = ValidAudiences;
        validIssuer = ValidIssuer;
        signingKeys = SigningKeys;
    }
}

public record MyJwtBearerSigningKeyOptions
{
    public MyJwtBearerSigningKeyOptions(string Id, string Issuer, string Value, int Length)
    {
        this.Id = Id;
        this.Issuer = Issuer;
        this.Value = Value;
        this.Length = Length;
    }

    public MyJwtBearerSigningKeyOptions()
    {
    }

    public string Id { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int Length { get; set; }

    public void Deconstruct(out string id, out string issuer, out string value, out int length)
    {
        id = Id;
        issuer = Issuer;
        value = Value;
        length = Length;
    }
}