using Articles.Domain.Constants;
using Articles.Shared.Monitoring;

namespace Articles.Application.AuthUseCases.Commands.Registration;

public sealed record RegistrationCommand(
	string UserName,
	string Email,
	string DomainId,
	string Password) : ICommand, IMetricsCommand
{
	public string CounterName => MetricsCounterNames.Registration;
}
