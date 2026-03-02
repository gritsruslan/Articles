using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class UsageLimitingOptions
{
	[Required]
	public UsageLimitingPolicy[] Policies { get; set; } = null!;
}
