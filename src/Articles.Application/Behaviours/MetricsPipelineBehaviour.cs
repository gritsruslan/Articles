using Articles.Application.Interfaces.Monitoring;
using Articles.Shared.Monitoring;
using MediatR;

namespace Articles.Application.Behaviours;

internal sealed class MetricsPipelineBehaviour<TRequest, TResponse>
	(IMetricsService metricsService)
	: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (request is not IMetricsRequest metricsRequest)
		{
			return await next(cancellationToken);
		}

		var counterName = metricsRequest.CounterName;
		try
		{
			var result = await next(cancellationToken);
			var resultBase = result as ResultBase ??
			                 throw new InvalidCastException($"Failed to convert the result to type ResultBase");

			if (resultBase.IsSuccess)
			{
				metricsService.MonitorSuccess(counterName);
			}
			else
			{
				// request was processed correctly but the user entered invalid data.
				metricsService.MonitorFailure(counterName);
			}
		}
		catch
		{
			metricsService.MonitorError(counterName);
			throw;
		}

		return await next(cancellationToken);
	}
}
