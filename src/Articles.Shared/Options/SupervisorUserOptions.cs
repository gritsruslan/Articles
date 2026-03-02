using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class SupervisorUserOptions
{
	public Guid Id { get; init; }

	[Required]
	public string Email { get; set; } = null!;

	[Required]
	public string Name { get; set; } = null!;

	[Required]
	public string DomainId { get; set; } = null!;

	[Required]
	public string Password { get; set; } = null!;
}
