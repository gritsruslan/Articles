using Articles.Domain.Identifiers;
using Articles.Domain.Models;

namespace Articles.Application.Interfaces.Repositories;

public interface ISessionRepository
{
	Task Add(Session session, CancellationToken cancellationToken);

	Task<Session?> GetById(SessionId sessionId, CancellationToken cancellationToken);

	Task DeleteById(SessionId sessionId, CancellationToken cancellationToken);
}
