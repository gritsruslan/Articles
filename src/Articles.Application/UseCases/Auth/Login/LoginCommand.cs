using Articles.Application.Authentication;
using Articles.Domain.Constants;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Auth.Login;

public sealed record LoginCommand(
	string Email,
	string Password,
	bool RememberMe,
	string UserAgent) : ICommand<AuthTokenPair>, IMetricsCommand, ITracedCommand
{
	public string CounterName => MetricNames.Login;
}
