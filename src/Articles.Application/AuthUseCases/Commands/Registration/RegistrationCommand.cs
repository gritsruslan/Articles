using Articles.Domain.Constants;
using Articles.Shared.Monitoring;

namespace Articles.Application.AuthUseCases.Commands.Registration;

public sealed record RegistrationCommand(
	string UserName,
	string Email,
	string DomainId,
	string Password) : ICommand, IMetricsCommand, ITracedCommand
{
	public string CounterName => MetricNames.Registration;
}
