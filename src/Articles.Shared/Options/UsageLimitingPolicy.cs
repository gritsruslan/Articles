using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class UsageLimitingPolicy
{
	[Required]
	public string Name { get; set; } = null!;

	public int OperationsCount { get; set; }

	public TimeSpan RefreshTime { get; set; }
}
