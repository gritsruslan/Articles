using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Domain.Constants;

namespace Articles.Infrastructure.Monitoring;

// Metrics per-use-case
internal sealed class UseCaseMetricsService(IMeterFactory meterFactory) : IUseCaseMetricsService
{
	private readonly Meter _meter = meterFactory.Create(OverallConstants.ApiName);

	private readonly ConcurrentDictionary<string, Counter<long>> _counters = new();

	public void MonitorSuccess(string name) => MonitorResult(name, "success");

	public void MonitorFailure(string name) => MonitorResult(name, "failure");

	public void MonitorError(string name) => MonitorResult(name, "error");

	private void MonitorResult(string name, string result)
	{
		var counter = GetCounter(name);
		counter.Add(1, new KeyValuePair<string, object?>("result", result));
	}

	private Counter<long> GetCounter(string name) =>
		_counters.GetOrAdd(name, newName => _meter.CreateCounter<long>(newName));
}
