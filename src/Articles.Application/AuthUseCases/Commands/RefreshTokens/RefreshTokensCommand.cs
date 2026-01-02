using Articles.Application.Authentication;

namespace Articles.Application.AuthUseCases.Commands.RefreshTokens;

public sealed record RefreshTokensCommand(string? RefreshToken, string UserAgent)
	: ICommand<AuthTokenPair>;
