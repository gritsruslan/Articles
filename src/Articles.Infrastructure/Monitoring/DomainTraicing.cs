using System.Diagnostics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Domain.Constants;

namespace Articles.Infrastructure.Monitoring;

public sealed class DomainTracing : ITracingSource
{
	public ActivitySource ActivitySource { get; } = new(OverallConstants.DomainName);
}
