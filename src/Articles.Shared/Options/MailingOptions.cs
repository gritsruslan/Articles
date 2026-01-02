using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class MailingOptions
{
	[Required]
	public required string HostEmail { get; init; } = null!;
}
