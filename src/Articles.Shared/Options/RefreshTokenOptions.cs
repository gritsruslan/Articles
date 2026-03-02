using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class RefreshTokenOptions
{
	[Required]
	public string Key { get; set; } = null!;

	[Required]
	public string Issuer { get; set; } = null!;

	[Required]
	public string Audience { get; set; } = null!;
}
