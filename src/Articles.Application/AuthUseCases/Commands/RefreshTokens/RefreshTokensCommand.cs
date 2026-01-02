using Articles.Application.Authentication;
using Articles.Shared.CQRS;

namespace Articles.Application.AuthUseCases.Commands.RefreshTokens;

public sealed record RefreshTokensCommand(string? RefreshToken, string UserAgent)
	: ICommand<AuthTokenPair>;
