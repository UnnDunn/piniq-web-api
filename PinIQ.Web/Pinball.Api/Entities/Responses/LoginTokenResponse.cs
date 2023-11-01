namespace Pinball.Api.Entities.Responses;

public record LoginTokenResponse(string AccessToken, string? RefreshToken = null);