using Articles.Domain.Identifiers;
using Articles.Domain.Models;
using Articles.Shared.Result;

namespace Articles.Application.Interfaces.Authentication;

public interface ISessionManager
{
	Session Create(UserId userId, string userAgent, bool longTermSupport);

	Session Create(UserId userId, string userAgent, DateTime expiresAt);

	Result Validate(Session session, UserId userId, string userAgent);
}
