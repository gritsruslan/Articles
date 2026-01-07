namespace Articles.Application.Interfaces.Monitoring;

public interface IMetricsService
{
	void MonitorSuccess(string name);

	void MonitorFailure(string name);

	void MonitorError(string name);
}
