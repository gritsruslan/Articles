using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class EmailConfirmationTokenOptions
{
	[Required]
	public string Key { get; set; } = null!;

	public TimeSpan LifeSpan { get; set; }
}
