namespace Articles.Application.Authorization;

internal interface IAuthorizedRequest
{
	public int RequiredPermissionId { get; }
}
