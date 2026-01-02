using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.CQRS;

namespace Articles.Application.AuthUseCases.Commands.Logout;

public sealed record LogoutCommand(string? RefreshToken) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int)DefaultPermissions.RequireAuthorization;
}
