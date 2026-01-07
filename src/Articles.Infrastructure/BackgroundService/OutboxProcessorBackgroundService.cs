using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Articles.Infrastructure.BackgroundService;

public class OutboxProcessorBackgroundService(
	ILogger<OutboxProcessorBackgroundService> logger,
	IServiceProvider serviceProvider,
	IOutboxMetricsService metricsService
	) : Microsoft.Extensions.Hosting.BackgroundService
{
	private const int BatchSize = 5;

	private const int MaxRetryCount = 3;

	private readonly TimeSpan _wait = TimeSpan.FromSeconds(5);

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
				var queueSize = await repository.QueueSize();
				var results = new List<ProcessOutboxMessageResult>(outboxMessages.Count);

				foreach (var outboxMessage in outboxMessages)
				{
					var result = await ProcessOutboxMessage(outboxMessage, publisher, stoppingToken);
					results.Add(result);
				}

				await repository.MarkAsProcessed(results, stoppingToken);
				metricsService.MonitorQueueSize(queueSize);
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
		IPublisher publisher,
		CancellationToken stoppingToken)
	{
		var start = Stopwatch.GetTimestamp();

		var result = ConvertOutboxMessageToDomainEvent(outboxMessage);
		if (result.IsFailure)
		{
			logger.LogError(
				result.Error.Message, // template
				result.Error.InvalidObjectValue); // property (OutboxMessageId)

			metricsService.MonitorProcessedMessage(success: false);

			return new ProcessOutboxMessageResult(outboxMessage, DateTime.UtcNow, result.Error.Message);
		}

		var domainEvent = result.Value;

		// retry logic
		int retriesLeft = MaxRetryCount;
		Exception? exception = null;
		while (retriesLeft > 0)
		{
			try
			{
				retriesLeft--;
				await publisher.Publish(domainEvent, stoppingToken);
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
		metricsService.MonitorRetries(MaxRetryCount - retriesLeft - 1);

		var elapsedMs =
			Stopwatch.GetElapsedTime(start).TotalMilliseconds;
		metricsService.MonitorMessageProcessingDuration(elapsedMs);

		if (exception is not null)
		{
			logger.LogError(exception, "Failed handling OutboxMessage with id = {id}", outboxMessage.Id);
			metricsService.MonitorProcessedMessage(success: false);
			return new ProcessOutboxMessageResult(outboxMessage, DateTime.UtcNow, exception.Message);
		}

		metricsService.MonitorProcessedMessage(success: true);
		return new ProcessOutboxMessageResult(outboxMessage, DateTime.UtcNow, exception?.ToString());
	}


	private Result<DomainEvent> ConvertOutboxMessageToDomainEvent(OutboxMessage outboxMessage)
	{
		DomainEvent? domainEvent;
		try
		{
			Type type = GetOrAddDomainEventType(outboxMessage.Type);
			domainEvent = JsonConvert.DeserializeObject(outboxMessage.ContentBlob, type) as DomainEvent;
		}
		catch (Exception)
		{
			return new Error(ErrorType.Failure,
				// template
				message: "Cannot parse outboxMessage with id =\"{outboxMessageId}\" to domain event",
				invalidObjectValueString: outboxMessage.Id.ToString());
		}

		if (domainEvent is null)
		{
			return new Error(ErrorType.Failure,
				message: "Empty or invalid outbox message with id =\"{outboxMessageId}\", cannot parse to domain event",
				invalidObjectValueString: outboxMessage.Id.ToString());
		}

		return domainEvent;
	}
}
