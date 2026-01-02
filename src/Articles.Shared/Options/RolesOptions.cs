using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class RolesOptions
{
	[Required]
	[MinLength(1)] // at least 1 for guest
	public RoleOption[] Roles { get; set; } = null!;
}
