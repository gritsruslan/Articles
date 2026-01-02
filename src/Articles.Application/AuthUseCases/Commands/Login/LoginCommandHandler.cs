using Articles.Application.Authentication;
using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Application.Interfaces.Security;

namespace Articles.Application.AuthUseCases.Commands.Login;

internal sealed class LoginCommandHandler(
	IUserRepository userRepository,
	ISessionRepository sessionRepository,
	IPasswordHasher passwordHasher,
	IAccessTokenManager accessTokenManager,
	IRefreshTokenManager refreshTokenManager,
	ISessionManager sessionManager) : ICommandHandler<LoginCommand, AuthTokenPair>
{
	public async Task<Result<AuthTokenPair>> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
	{
		var emailResult = Email.Create(request.Email);
		if (emailResult.IsFailure)
		{
			return SecurityErrors.IncorrectEmailOrPassword();
		}

		var email = emailResult.Value;
		var password = request.Password;

		var user = await userRepository.GetByEmail(email, cancellationToken);
		if (user is null)
		{
			return SecurityErrors.IncorrectEmailOrPassword();
		}

		var passwordMatches =
			passwordHasher.VerifyPassword(user.PasswordHash, user.Salt, password);
		if (!passwordMatches)
		{
			return SecurityErrors.IncorrectEmailOrPassword();
		}

		var session = sessionManager.Create(user.Id, request.UserAgent, request.RememberMe);
		await sessionRepository.Add(session, cancellationToken);

		var accessTokenStr = await accessTokenManager.CreateEncrypted(user.Id, cancellationToken);
		var refreshTokenStr =
			await refreshTokenManager.CreateEncrypted(user.Id, session.Id, cancellationToken);

		return new AuthTokenPair(accessTokenStr, refreshTokenStr);
	}
}
