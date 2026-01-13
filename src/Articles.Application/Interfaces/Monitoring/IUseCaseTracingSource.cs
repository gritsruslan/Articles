using System.Diagnostics;

namespace Articles.Application.Interfaces.Monitoring;

public interface IUseCaseTracingSource
{
	ActivitySource ActivitySource { get; }
}

public interface IOutboxTracingSource
{
	ActivitySource ActivitySource { get; }
}
