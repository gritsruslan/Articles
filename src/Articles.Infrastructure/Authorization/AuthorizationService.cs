using Articles.Application.Authorization;
using Articles.Application.Interfaces.Authentication;
using Articles.Domain.Errors;
using Articles.Domain.Permissions;
using Articles.Shared.Result;

namespace Articles.Infrastructure.Authorization;

internal sealed class AuthorizationService(IApplicationUserProvider applicationUserProvider) : IAuthorizationService
{
	public Result IsAllowed(int permissionId)
	{
		var currentUser = applicationUserProvider.CurrentUser;

		if (currentUser.IsGuest)
		{
			return SecurityErrors.Unauthorized();
		}

		if (permissionId == (int)DefaultPermissions.RequireAuthorization ||
		    currentUser.Permissions.Any(p => p.Id == permissionId))
		{
			return Result.Success();
		}

		return SecurityErrors.Forbidden();
	}
}
