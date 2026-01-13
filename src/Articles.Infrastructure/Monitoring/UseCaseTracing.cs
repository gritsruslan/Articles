using System.Diagnostics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Domain.Constants;

namespace Articles.Infrastructure.Monitoring;

public sealed class UseCaseTracing : IUseCaseTracingSource
{
	public ActivitySource ActivitySource { get; } = new(OverallConstants.ApiName);
}
