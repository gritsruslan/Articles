using Articles.Domain.DomainEvents;

namespace Articles.Application.Interfaces.Repositories;

public interface IDomainEventRepository
{
	Task Add(DomainEvent domainEvent, CancellationToken cancellationToken);

	Task<IEnumerable<OutboxMessage>> GetUnprocessed(int take, CancellationToken cancellationToken);

	Task<int> GetQueueSize();

	Task MarkAsProcessed(
		IEnumerable<ProcessOutboxMessageResult> processResults,
		CancellationToken cancellationToken);
}
