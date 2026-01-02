using Articles.Shared.CQRS;

namespace Articles.Application.AuthUseCases.Commands.Registration;

public sealed record RegistrationCommand(
	string UserName,
	string Email,
	string DomainId,
	string Password) : ICommand;
