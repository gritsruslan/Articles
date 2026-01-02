namespace Articles.Storage.Postgres.Entities;

public sealed class UserEntity
{
	public Guid Id { get; set; }

	public required string Email { get; set; }

	public required string Name { get; set; }

	public int RoleId { get; set; }

	public required string DomainId { get; set; }

	public bool EmailConfirmed { get; set; }

	public required byte[] PasswordHash { get; set; }

	public required byte[] Salt { get; set; }

	public ICollection<SessionEntity> Sessions { get; set; } = [];
}
