using Articles.Domain.Identifiers;

namespace Articles.Domain.Models;

public sealed class Role
{
	public RoleId Id { get; init; }

	public required string Name { get; init; }

	public required IReadOnlyCollection<Permission> Permissions { get; init; }
}
