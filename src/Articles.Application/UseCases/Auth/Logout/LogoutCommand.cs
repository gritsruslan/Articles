using Articles.Application.Authorization;
using Articles.Domain.Constants;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Auth.Logout;

public sealed record LogoutCommand(string? RefreshToken) : ICommand, IAuthorizedCommand, IMetricsCommand, ITracedCommand
{
	public int RequiredPermissionId => (int)DefaultPermissions.RequireAuthorization;

	public string CounterName => MetricNames.Logout;
}
