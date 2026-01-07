using Articles.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Articles.Application.NotificationHandlers;

public class TestEventNotificationHandler(
	ILogger<TestEventNotificationHandler> logger) : INotificationHandler<TestDomainEvent>
{
	public async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
	{
		await Task.Delay(Random.Shared.Next(0, 200), cancellationToken);
		throw new Exception("Something went wrong :((((");
	}
}
