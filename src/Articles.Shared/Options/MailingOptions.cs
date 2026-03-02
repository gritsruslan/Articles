using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class MailingOptions
{
	[Required]
	public string HostEmail { get; set; } = null!;
}
