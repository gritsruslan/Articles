namespace Articles.Domain.Models;

public sealed class Permission
{
	public int Id { get; init; }

	public required string Name { get; init; }
}
