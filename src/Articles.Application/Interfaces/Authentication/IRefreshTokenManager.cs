using Articles.Application.Authentication;

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
