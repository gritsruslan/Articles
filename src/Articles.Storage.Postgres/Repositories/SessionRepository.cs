using Articles.Storage.Postgres.Entities;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class SessionRepository(ArticlesDbContext dbContext) : ISessionRepository
{
	public Task Add(Session session, CancellationToken cancellationToken)
	{
		var sessionEntity = new SessionEntity
		{
			Id = session.Id.Value,
			UserId = session.UserId.Value,
			UserAgent = session.UserAgent,
			IssuedAt = session.IssuedAt,
			ExpiresAt = session.ExpiresAt
		};

		dbContext.Sessions.Add(sessionEntity);
		return dbContext.SaveChangesAsync(cancellationToken);
	}

	public Task<Session?> GetById(SessionId sessionId, CancellationToken cancellationToken)
	{
		return dbContext.Sessions
			.Where(s => s.Id == sessionId.Value)
			.Select(s => new Session
			{
				Id = SessionId.Create(s.Id),
				UserId = UserId.Create(s.UserId),
				UserAgent = s.UserAgent,
				IssuedAt = s.IssuedAt,
				ExpiresAt = s.ExpiresAt
			})
			.FirstOrDefaultAsync(cancellationToken);
	}

	public Task<bool> AnyByUserId(UserId userId, CancellationToken cancellationToken) =>
		dbContext.Sessions.AnyAsync(s => s.UserId == userId.Value, cancellationToken);

	public Task DeleteById(SessionId sessionId, CancellationToken cancellationToken) =>
		dbContext.Sessions
			.Where(s => s.Id == sessionId.Value)
			.ExecuteDeleteAsync(cancellationToken);
}
