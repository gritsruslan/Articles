using System.Diagnostics.Metrics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Domain.Constants;

namespace Articles.Infrastructure.Monitoring;

// metrics for outbox processor
public class OutboxMetricsService : IOutboxMetricsService
{
	private readonly Counter<long> _processedMessages;

	private readonly Counter<long> _retryAttempts;

	private readonly Gauge<int> _queueSizeGauge;

	private readonly Histogram<double> _messageProcessingDuration;

	public OutboxMetricsService(IMeterFactory meterFactory)
	{
		var meter = meterFactory.Create(OverallConstants.Outbox);

		_queueSizeGauge = meter.CreateGauge<int>(MetricNames.OutboxProcessorQueueSize);

		_messageProcessingDuration = meter.CreateHistogram<double>(
			MetricNames.OutboxMessageProcessingDuration);

		_processedMessages = meter.CreateCounter<long>(
			MetricNames.OutboxProcessedMessages);

		_retryAttempts = meter.CreateCounter<long>(MetricNames.OutboxRetryAttempts);
	}

	public void MonitorProcessedMessage(bool success) =>
		_processedMessages.Add(1, new KeyValuePair<string, object?>("success", success));

	public void MonitorRetries(long count) => _retryAttempts.Add(count);

	public void MonitorQueueSize(int size) => _queueSizeGauge.Record(size);

	public void MonitorMessageProcessingDuration(double duration) =>
		_messageProcessingDuration.Record(duration);
}
