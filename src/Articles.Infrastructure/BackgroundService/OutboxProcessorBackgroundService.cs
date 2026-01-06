using System.Collections.Concurrent;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Articles.Infrastructure.BackgroundService;

public class OutboxProcessorBackgroundService(
	ILogger<OutboxProcessorBackgroundService> logger,
	IServiceProvider serviceProvider
	) : Microsoft.Extensions.Hosting.BackgroundService
{
	private const int BatchSize = 5;

	private const int MaxRetryCount = 3;

	private readonly TimeSpan _wait = TimeSpan.FromSeconds(1);

	private static readonly ConcurrentDictionary<string, Type> DomainEventTypesDictionary = new();

	// for caching
	// needs AssemblyQualifiedName
	private static Type GetOrAddDomainEventType(string typename)
	{
		return DomainEventTypesDictionary.GetOrAdd(typename, typeName => Type.GetType(typeName) ??
		   throw new TypeLoadException($"Could not find type '{typeName}' for DomainEvent"));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("=== Starting OutboxProcessorBackgroundService ===");

		await Task.Yield(); // not wait

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				await using var scope = serviceProvider.CreateAsyncScope();
				var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
				var repository = scope.ServiceProvider.GetRequiredService<IDomainEventRepository>();

				var outboxMessages = await repository.GetUnprocessed(BatchSize, stoppingToken);
				var results = new List<ProcessOutboxMessageResult>(outboxMessages.Count);

				foreach (var outboxMessage in outboxMessages)
				{
					var result = await ProcessOutboxMessage(outboxMessage, publisher);
					results.Add(result);
				}

				await repository.MarkAsProcessed(results, CancellationToken.None);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unhandled exception in OutboxMessagesBackgroundService");
			}

			await Task.Delay(_wait, stoppingToken);
		}
	}

	private async Task<ProcessOutboxMessageResult> ProcessOutboxMessage(
		OutboxMessage outboxMessage,
		IPublisher publisher)
	{
		DomainEvent? domainEvent;
		try
		{
			Type type = GetOrAddDomainEventType(outboxMessage.Type);
			domainEvent = JsonConvert.DeserializeObject(outboxMessage.ContentBlob, type) as DomainEvent;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Cannot parse outboxMessage with id={outboxMessageid} to domainEvent", outboxMessage.Id);
			return new ProcessOutboxMessageResult(outboxMessage.Id, DateTime.UtcNow, ex.Message);
		}

		if (domainEvent is null)
		{
			return new ProcessOutboxMessageResult(
				outboxMessage.Id,
				DateTime.UtcNow,
				"Empty or invalid outbox message, cannot parse to domain Event");
		}

		// retry logic
		int retriesLeft = MaxRetryCount;
		Exception? exception = null;
		while (retriesLeft > 0)
		{
			try
			{
				retriesLeft--;
				await publisher.Publish(domainEvent, CancellationToken.None);
				break;
			}
			catch (Exception ex)
			{
				if (retriesLeft == 0)
				{
					exception = ex;
				}
			}
		}

		if (exception is not null)
		{
			logger.LogError(exception, "Failed handling OutboxMessage with id= {id}", outboxMessage.Id);
		}

		return new ProcessOutboxMessageResult(outboxMessage.Id, DateTime.UtcNow, exception?.ToString());
	}
}
