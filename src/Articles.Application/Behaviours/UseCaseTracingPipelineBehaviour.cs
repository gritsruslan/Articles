using System.Diagnostics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Shared.Monitoring;
using MediatR;

namespace Articles.Application.Behaviours;

internal sealed class UseCaseTracingPipelineBehaviour<TRequest, TResponse>(
	ITracingSource tracingSource) :
	IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (request is not ITracedRequest)
		{
			return await next(cancellationToken);
		}

		using var activity = tracingSource.ActivitySource
			.StartActivity("usecase", ActivityKind.Internal, default(ActivityContext));
		activity?.AddTag("articles.request", request.GetType().Name);

		try
		{
			var overallResult = await next(cancellationToken);
			var result = overallResult as ResultBase ?? throw new InvalidCastException();

			if (result.IsSuccess)
			{
				activity?.AddTag("result", "success");
			}
			else
			{
				activity?.AddTag("result", "failure");
			}

			return overallResult;
		}
		catch
		{
			activity?.AddTag("result", "error");
			throw;
		}
	}
}
