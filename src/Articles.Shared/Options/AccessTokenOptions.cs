using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class AccessTokenOptions
{
	[Required]
	public required string Key { get; init; } = null!;

	[Required]
	public required string Issuer { get; init; } = null!;

	[Required]
	public required string Audience { get; init; } = null!;

	[Required]
	public TimeSpan LifeSpan { get; init; }
}
