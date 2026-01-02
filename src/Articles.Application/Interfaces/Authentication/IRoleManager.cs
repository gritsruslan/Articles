using Articles.Domain.Identifiers;
using Articles.Domain.Models;

namespace Articles.Application.Interfaces.Authentication;

public interface IRoleManager
{
	Role GetRole(RoleId roleId);

	Role Guest();
}
