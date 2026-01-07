namespace Articles.Application.Interfaces.Monitoring;

public interface IOutboxMetricsService
{
	void MonitorQueueSize(int batchSize);

	void MonitorProcessedMessage(bool success);

	void MonitorMessageProcessingDuration(double duration);

	void MonitorRetries(long count);
}
