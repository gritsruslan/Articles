using System.Text.Json.Serialization;
using Articles.Domain.Enums;

namespace Articles.Application.Authentication;

public class RecognizedUser
{
	public UserId Id { get; init; }

	public Email? Email { get; init; }

	public bool? EmailConfirmed { get; init; }

	public required Role Role { get; init; }

	[JsonIgnore]
	public IReadOnlyCollection<Permission> Permissions => Role.Permissions;

	public bool IsGuest => Role.Id.Value == (int)Roles.Guest;
}
