using System.Diagnostics;

namespace Articles.Application.Interfaces.Monitoring;

public interface ITracingSource
{
	ActivitySource ActivitySource { get; }
}
