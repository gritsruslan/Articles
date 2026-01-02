namespace Articles.Storage.Postgres.Entities;

public sealed class SessionEntity
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; }

	public UserEntity User { get; set; } = null!;

	public required string UserAgent { get; set; }

	public DateTime IssuedAt { get; set; }

	public DateTime ExpiresAt { get; set; }
}
