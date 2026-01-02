using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class UsageLimitingPolicy
{
	[Required]
	public required string Name { get; init; } = null!;

	public int OperationsCount { get; init; }

	public TimeSpan RefreshTime { get; init; }
}
