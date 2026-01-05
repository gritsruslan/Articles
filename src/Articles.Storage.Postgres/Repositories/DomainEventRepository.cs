using Articles.Domain.DomainEvents;
using Articles.Storage.Postgres.Entities;
using Newtonsoft.Json;

namespace Articles.Storage.Postgres.Repositories;

public sealed class DomainEventRepository(ArticlesDbContext dbContext) : IDomainEventRepository
{
	public async Task Add(DomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var outboxMessage = new OutboxMessageEntity
		{
			Id = Guid.NewGuid(),
			EmittedAt = DateTime.UtcNow,
			Type = domainEvent.GetType().AssemblyQualifiedName ??
			       throw new ArgumentException("AssemblyQualifiedName of DomainEvent is null"),
			ContentBlob = JsonConvert.SerializeObject(domainEvent)
		};

		dbContext.OutboxMessages.Add(outboxMessage);
		await dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<IList<OutboxMessage>> GetUnprocessed(int take, CancellationToken cancellationToken)
	{
		var messages = await dbContext.OutboxMessages
			.Where(m => m.ProcessedAt == null)
			.OrderBy(m => m.EmittedAt)
			.Take(take)
			.ToListAsync(cancellationToken);

		return messages.Select(m => new OutboxMessage
		{
			Id = m.Id,
			EmittedAt = m.EmittedAt,
			ProcessedAt = m.ProcessedAt,
			ContentBlob = m.ContentBlob,
			Type = m.Type,
			Error = m.Error,
		}).ToList();
	}

	// not the best
	public async Task MarkAsProcessed(
		IList<ProcessOutboxMessageResult> processResults,
		CancellationToken cancellationToken)
	{
		var ids = processResults.Select(p => p.Id);

		await dbContext.OutboxMessages.Where(m => ids.Contains(m.Id))
			.ExecuteUpdateAsync(
				s =>
					s.SetProperty(
						x => x.ProcessedAt,
						x => processResults.First(r => r.Id == x.Id).ProcessedAt)
					.SetProperty(
						x => x.Error,
						x => processResults.First(r => r.Id == x.Id).Error)
				, cancellationToken);
	}
}
