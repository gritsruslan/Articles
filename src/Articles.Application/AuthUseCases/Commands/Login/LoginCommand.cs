using Articles.Application.Authentication;
using Articles.Domain.Constants;
using Articles.Shared.Monitoring;

namespace Articles.Application.AuthUseCases.Commands.Login;

public sealed record LoginCommand(
	string Email,
	string Password,
	bool RememberMe,
	string UserAgent) : ICommand<AuthTokenPair>, IMetricsCommand
{
	public string CounterName => MetricNames.Login;
}
