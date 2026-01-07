using Articles.Application.Authorization;
using Articles.Domain.Constants;
using Articles.Domain.Permissions;
using Articles.Shared.Monitoring;

namespace Articles.Application.AuthUseCases.Commands.Logout;

public sealed record LogoutCommand(string? RefreshToken) : ICommand, IAuthorizedCommand, IMetricsCommand
{
	public int RequiredPermissionId => (int)DefaultPermissions.RequireAuthorization;

	public string CounterName => MetricNames.Logout;
}
