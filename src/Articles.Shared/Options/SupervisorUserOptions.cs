using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class SupervisorUserOptions
{
	public Guid Id { get; init; }

	[Required]
	public required string Email { get; init; } = null!;

	[Required]
	public required string Name { get; init; } = null!;

	[Required]
	public required string DomainId { get; init; } = null!;

	// todo : get from secrets
	[Required]
	public required string Password { get; init; } = null!;
}
