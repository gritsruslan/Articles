using Articles.Domain.Models;
using Articles.Shared.DefaultServices;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging;

namespace Articles.Infrastructure.Authentication;

internal sealed class SessionManager(
	IOptions<SessionOptions> options,
	ILogger<SessionManager> logger,
	IDateTimeProvider dateTimeProvider) : ISessionManager
{
	public Session Create(UserId userId, string userAgent, bool longTermSupport)
	{
		var opt = options.Value;
		var dateTime = dateTimeProvider.UtcNow;
		var expiresAt = longTermSupport
			? dateTime.AddMonths(opt.LongTermLifeSpanMonths)
			: dateTime.AddHours(opt.ShortTermLifeSpanHours);

		return new Session
		{
			Id = SessionId.New(),
			UserId = userId,
			UserAgent = userAgent,
			IssuedAt = dateTime,
			ExpiresAt = expiresAt
		};
	}

	public Session Create(UserId userId, string userAgent, DateTime expiresAt)
	{
		return new Session
		{
			Id = SessionId.New(),
			UserId = userId,
			UserAgent = userAgent,
			IssuedAt = dateTimeProvider.UtcNow,
			ExpiresAt = expiresAt
		};
	}

	public Result Validate(Session session, UserId userId, string userAgent)
	{
		if (userId != session.UserId)
		{
			logger.LogWarning("The UserID in the token and in the session do not match, " +
			                  "maybe someone has stolen refresh token. UserId in token : {token_user_id} ," +
			                  "UserId in session : {session_user_id} .", userId.Value, session.UserId.Value);
			return SecurityErrors.Unauthorized();
		}

		if (userAgent != session.UserAgent)
		{
			logger.LogWarning("The UserAgent in the session and in the client dont match," +
			                  " maybe someone has stolen refresh token." +
			                  "UserId is : {user_id} \n" +
			                  "Session UserAgent is : {session_user_agent} \n" +
			                  "Client UserAgent is : {client_user_agent}",
				userId.Value, session.UserAgent, userAgent);
			return SecurityErrors.Unauthorized();
		}

		if (dateTimeProvider.UtcNow > session.ExpiresAt)
		{
			return SecurityErrors.RefreshTokenExpired();
		}

		return Result.Success();
	}
}
