using Articles.Application.Interfaces.Monitoring;
using Articles.Shared.Monitoring;
using MediatR;

namespace Articles.Application.Behaviours;

internal sealed class MetricsPipelineBehaviour<TRequest, TResponse>
	(IUseCaseMetricsService useCaseMetricsService)
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
		TResponse result;
		try
		{
			result = await next(cancellationToken);
			var resultBase = result as ResultBase ??
			                 throw new InvalidCastException($"Failed to convert the result to type ResultBase");

			if (resultBase.IsSuccess)
			{
				useCaseMetricsService.MonitorSuccess(counterName);
			}
			else
			{
				// request was processed correctly but the user entered invalid data.
				useCaseMetricsService.MonitorFailure(counterName);
			}
		}
		catch
		{
			useCaseMetricsService.MonitorError(counterName);
			throw;
		}

		return result;
	}
}
