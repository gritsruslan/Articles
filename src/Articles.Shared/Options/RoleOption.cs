using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class RoleOption
{
	[Required]
	public string Name { get; set; } = null!;

	[Required]
	public PermissionOptions Permissions { get; set; } = null!;
}
