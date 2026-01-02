using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class UsageLimitingOptions
{
	[Required]
	public required UsageLimitingPolicy[] Policies { get; init; } = null!;
}
