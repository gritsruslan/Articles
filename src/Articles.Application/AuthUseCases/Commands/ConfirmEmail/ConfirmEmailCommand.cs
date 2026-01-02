using Articles.Shared.CQRS;

namespace Articles.Application.AuthUseCases.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string EmailConfirmationToken) : ICommand;
