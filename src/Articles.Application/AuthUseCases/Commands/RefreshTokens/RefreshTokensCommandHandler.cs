using Articles.Application.Authentication;
using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.DefaultServices;
using Articles.Shared.UnitOfWork;

namespace Articles.Application.AuthUseCases.Commands.RefreshTokens;

internal sealed class RefreshTokensCommandHandler(
	ISessionRepository sessionRepository,
	IAccessTokenManager accessTokenManager,
	IRefreshTokenManager refreshTokenManager,
	ISessionManager sessionManager,
	IUnitOfWork unitOfWork,
	IDateTimeProvider dateTimeProvider) : ICommandHandler<RefreshTokensCommand, AuthTokenPair>
{
	public async Task<Result<AuthTokenPair>> Handle(
		RefreshTokensCommand request,
		CancellationToken cancellationToken)
	{
		var refreshTokenResult = await refreshTokenManager.Decrypt(request.RefreshToken, cancellationToken);
		if (refreshTokenResult.IsFailure)
		{
			return refreshTokenResult.Error;
		}

		var refreshToken = refreshTokenResult.Value;
		var validationResult = refreshTokenManager.Validate(refreshToken);
		if (validationResult.IsFailure)
		{
			return validationResult.Error;
		}

		var session = await sessionRepository.GetById(refreshToken.SessionId, cancellationToken);
		if (session is null)
		{
			return SecurityErrors.SessionExpired();
		}

		var sessionValidation = sessionManager.Validate(session, refreshToken.UserId, request.UserAgent);
		if (sessionValidation.IsFailure)
		{
			return sessionValidation.Error;
		}

		var expiresAt = dateTimeProvider.UtcNow + (session.ExpiresAt - session.IssuedAt);
		var newSession = sessionManager.Create(refreshToken.UserId, request.UserAgent, expiresAt);

		var newAccessToken = await accessTokenManager.CreateEncrypted(refreshToken.UserId, cancellationToken);
		var newRefreshToken =
			await refreshTokenManager.CreateEncrypted(refreshToken.UserId, newSession.Id, cancellationToken);

		await using var scope = await unitOfWork.StartScope(cancellationToken);

		await sessionRepository.DeleteById(session.Id, cancellationToken);
		await sessionRepository.Add(newSession, cancellationToken);

		await scope.Commit(cancellationToken);

		return new AuthTokenPair(newAccessToken, newRefreshToken);
	}
}
