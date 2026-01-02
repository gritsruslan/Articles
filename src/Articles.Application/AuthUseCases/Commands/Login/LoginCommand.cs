using Articles.Application.Authentication;
using Articles.Shared.CQRS;

namespace Articles.Application.AuthUseCases.Commands.Login;

public sealed record LoginCommand(
	string Email,
	string Password,
	bool RememberMe,
	string UserAgent) : ICommand<AuthTokenPair>;
