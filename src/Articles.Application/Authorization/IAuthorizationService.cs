using Articles.Shared.Result;

namespace Articles.Application.Authorization;

public interface IAuthorizationService
{
	Result IsAllowed(int permissionId);
}
