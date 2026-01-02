using Articles.Domain.Identifiers;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Models;

public sealed class User
{
	public UserId Id { get; set; }

	public UserName Name { get; set; }

	public Email Email { get; set; }

	public bool EmailConfirmed { get; set; }

	public RoleId RoleId { get; set; }

	public DomainId DomainId { get; set; }

	public required byte[] PasswordHash { get; set; }

	public required byte[] Salt { get; set; }
}
