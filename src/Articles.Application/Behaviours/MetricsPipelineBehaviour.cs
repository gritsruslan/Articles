using Articles.Application.Interfaces.Monitoring;
using MediatR;

namespace Articles.Application.Behaviours;

internal sealed class MetricsPipelineBehaviour<TRequest, TResponse>
	(IMetricsService metricsService)
	: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	public Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
