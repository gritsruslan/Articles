using System.Diagnostics;
using Articles.Application.Interfaces.Monitoring;
using Articles.Shared.Monitoring;
using MediatR;

namespace Articles.Application.Behaviours;

internal sealed class UseCaseTracingPipelineBehaviour<TRequest, TResponse>(
	IUseCaseTracingSource useCaseTracingSource) :
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

		using var activity = useCaseTracingSource.ActivitySource
			.StartActivity("Articles.UseCase", ActivityKind.Internal, default(ActivityContext));
		activity?.AddTag("request.type", request.GetType().Name);

		try
		{
			var overallResult = await next(cancellationToken);
			var result = overallResult as ResultBase ?? throw new InvalidCastException();

			activity?.AddTag("result", result.IsSuccess ? "success" : "failure");

			return overallResult;
		}
		catch
		{
			activity?.AddTag("result", "error");
			throw;
		}
	}
}
