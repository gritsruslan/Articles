using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Errors;
using Articles.Shared.CQRS;
using Articles.Shared.Result;

namespace Articles.Application.AuthUseCases.Commands.Logout;

internal sealed class LogoutCommandHandler(
	IRefreshTokenManager refreshTokenManager,
	IApplicationUserProvider userProvider,
	ISessionRepository sessionRepository) : ICommandHandler<LogoutCommand>
{
	public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
	{
		var refreshTokenStr = request.RefreshToken;
		var refreshTokenResult = await refreshTokenManager.Decrypt(refreshTokenStr, cancellationToken);
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
			return SecurityErrors.Unauthorized();
		}

		await sessionRepository.DeleteById(refreshToken.SessionId, cancellationToken);


		return Result.Success();
	}
}
