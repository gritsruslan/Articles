using System.Diagnostics;
using Articles.Domain.DomainEvents;
using Articles.Shared.DefaultServices;
using Articles.Storage.Postgres.Entities;
using Newtonsoft.Json;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class DomainEventRepository(ArticlesDbContext dbContext, IDateTimeProvider dateTimeProvider) : IDomainEventRepository
{
	public async Task Add(DomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var outboxMessage = new OutboxMessage
		{
			Id = Guid.NewGuid(),
			EmittedAt = dateTimeProvider.UtcNow,
			Type = domainEvent.GetType().AssemblyQualifiedName ??
			       throw new ArgumentException("AssemblyQualifiedName of DomainEvent is null"),
			ContentBlob = JsonConvert.SerializeObject(domainEvent),
			TraceId = Activity.Current?.TraceId.ToHexString(),
			SpanId = Activity.Current?.SpanId.ToHexString()
		};

		dbContext.OutboxMessages.Add(outboxMessage);
		await dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<IEnumerable<OutboxMessage>> GetUnprocessed(int take, CancellationToken cancellationToken)
	{
		return await dbContext.OutboxMessages
			.AsTracking() //enable tracking for batch update
			.Where(m => m.ProcessedAt == null)
			.OrderBy(m => m.EmittedAt)
			.Take(take)
			.ToListAsync(cancellationToken);
	}

	public async Task<int> GetQueueSize() =>
		await dbContext.OutboxMessages.Where(m => m.ProcessedAt == null).CountAsync();

	// batch update (not really)
	public async Task MarkAsProcessed(
		IEnumerable<ProcessOutboxMessageResult> processResults,
		CancellationToken cancellationToken)
	{
		foreach (var result in processResults)
		{
			var message = result.Message;

			message.ProcessedAt = result.ProcessedAt;
			message.Error = result.Error;
		}

		await dbContext.SaveChangesAsync(cancellationToken);
	}
}
