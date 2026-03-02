using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class SmtpClientOptions
{
	[Required]
	public string Host { get; set; } = null!;

	public int Port { get; set; }
}
