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

		const string useCaseActivityName = "Articles.UseCase";
		using var activity = useCaseTracingSource.ActivitySource
			.StartActivity(useCaseActivityName, ActivityKind.Internal, default(ActivityContext));
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
