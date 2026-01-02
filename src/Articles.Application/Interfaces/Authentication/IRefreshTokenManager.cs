using Articles.Application.Authentication;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Application.Interfaces.Authentication;

public interface IRefreshTokenManager
{
	Task<string> CreateEncrypted(
		UserId userId,
		SessionId sessionId,
		CancellationToken cancellationToken);
	Result Validate(RefreshToken refreshToken);

	Task<Result<RefreshToken>> Decrypt(string? refreshTokenStr, CancellationToken cancellationToken);
}
