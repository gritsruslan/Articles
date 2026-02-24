using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Auth.ConfirmEmail;

public sealed record ConfirmEmailCommand(string EmailConfirmationToken) : ICommand;
