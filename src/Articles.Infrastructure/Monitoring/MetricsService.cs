using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Articles.Application.Interfaces.Monitoring;

namespace Articles.Infrastructure.Monitoring;


public class MetricsService(IMeterFactory meterFactory) : IMetricsService
{
	private readonly Meter _meter = meterFactory.Create("Articles");

	private readonly ConcurrentDictionary<string, Counter<long>> _counters = new();

	public void MonitorSuccess(string name)
	{
		GetCounter(name).Add(1, new KeyValuePair<string, object?>("result", "success"));
	}

	public void MonitorFailure(string name)
	{
		GetCounter(name).Add(1, new KeyValuePair<string, object?>("result", "failure"));
	}

	public void MonitorError(string name)
	{
		GetCounter(name).Add(1, new KeyValuePair<string, object?>("result", "error"));
	}

	private Counter<long> GetCounter(string name) =>
		_counters.GetOrAdd(name, newName => _meter.CreateCounter<long>(newName));
}
