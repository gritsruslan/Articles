namespace Articles.Shared.Monitoring;

public interface IMetricsRequest
{
	string CounterName { get; }
}

public interface IMetricsQuery : IMetricsRequest;

public interface IMetricsCommand : IMetricsRequest;
