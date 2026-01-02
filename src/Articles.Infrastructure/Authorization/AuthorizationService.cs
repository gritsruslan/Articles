using Articles.Application.Authorization;
using Articles.Domain.Permissions;

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
