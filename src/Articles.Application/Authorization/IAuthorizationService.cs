namespace Articles.Application.Authorization;

public interface IAuthorizationService
{
	Result IsAllowed(int permissionId);
}
