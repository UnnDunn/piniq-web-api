namespace Pinball.Api.Entities.Configuration;

public record ApplicationInsightsOptions(bool IsEnabled = false)
{
    public bool IsEnabled { get; set; } = IsEnabled;
}

public record DeveloperOptions
{
    public string? LoginTestKey { get; set; }
}