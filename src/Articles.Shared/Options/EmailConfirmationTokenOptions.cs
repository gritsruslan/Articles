using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class EmailConfirmationTokenOptions
{
	[Required]
	public required string Key { get; init; } = null!;

	public TimeSpan LifeSpan { get; init; }
}
