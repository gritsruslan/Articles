using System.Diagnostics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Domain.Constants;

namespace Articles.Infrastructure.Monitoring;

internal sealed class OutboxTracing : IOutboxTracingSource
{
	public ActivitySource ActivitySource { get; } = new(OverallConstants.Outbox);
}
