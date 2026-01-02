using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class SmtpClientOptions
{
	[Required]
	public required string Host { get; init; } = null!;

	public int Port { get; init; }
}
