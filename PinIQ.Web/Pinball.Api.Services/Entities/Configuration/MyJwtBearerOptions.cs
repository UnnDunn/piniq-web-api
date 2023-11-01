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

    public IList<string> ValidAudiences { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public IList<MyJwtBearerSigningKeyOptions> SigningKeys { get; set; } = null!;
    
    public int AccessTokenExpirationMinutes { get; set; }
    
    public int RefreshTokenExpirationDays { get; set; }
    
    public int RefreshTokenRenewalWindowDays { get; set; }
    
    public MyJwtBearerOptions() {}

    public void Deconstruct(out IList<string> validAudiences, out string validIssuer, out IList<MyJwtBearerSigningKeyOptions> signingKeys)
    {
        validAudiences = this.ValidAudiences;
        validIssuer = this.ValidIssuer;
        signingKeys = this.SigningKeys;
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

    public string Id { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int Length { get; set; }

    public MyJwtBearerSigningKeyOptions() {}
    
    public void Deconstruct(out string id, out string issuer, out string value, out int length)
    {
        id = this.Id;
        issuer = this.Issuer;
        value = this.Value;
        length = this.Length;
    }
}