using Articles.Domain.Identifiers;

namespace Articles.Domain.Models;

public sealed class Session
{
	public SessionId Id { get; set; }

	public UserId UserId { get; set; }

	public required string UserAgent { get; set; }

	public DateTime IssuedAt { get; set; }

	public DateTime ExpiresAt { get; set; }
}
