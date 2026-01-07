using Articles.Domain.DomainEvents;

namespace Articles.Application.Interfaces.Repositories;

public interface IDomainEventRepository
{
	Task Add(DomainEvent domainEvent, CancellationToken cancellationToken);

	Task<IList<OutboxMessage>> GetUnprocessed(int take, CancellationToken cancellationToken);

	Task<int> QueueSize();

	Task MarkAsProcessed(
		IList<ProcessOutboxMessageResult> processResults,
		CancellationToken cancellationToken);
}
