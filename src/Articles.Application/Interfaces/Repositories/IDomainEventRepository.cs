using Articles.Domain.DomainEvents;

namespace Articles.Application.Interfaces.Repositories;

public interface IDomainEventRepository
{
	Task Add(DomainEvent domainEvent, CancellationToken cancellationToken);

	Task<OutboxMessage> GetUnprocessed(int take, CancellationToken cancellationToken);

	Task MarkAsProcessed(Guid id, CancellationToken cancellationToken);
}
