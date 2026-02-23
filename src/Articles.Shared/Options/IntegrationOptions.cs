namespace Articles.Shared.Options;

public sealed class IntegrationOptions
{
	public required string[] AllowedOrigins { get; init; } = null!;
}
