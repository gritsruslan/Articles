using Articles.Application.Authentication;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Auth.RefreshTokens;

public sealed record RefreshTokensCommand(string? RefreshToken, string UserAgent)
	: ICommand<AuthTokenPair>;
