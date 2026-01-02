using System.Diagnostics;
using Articles.Application.Authorization;
using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Application.UsageLimiting;
using Articles.Shared.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace Articles.Application.Behaviours;

internal sealed class UsageLimitingPipelineBehaviour<TRequest, TResponse>(
	IApplicationUserProvider userProvider,
	IUsageLimitingRepository limitingRepository,
	IOptions<UsageLimitingOptions> limitingOptions)
	: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (request is not IUsageLimitedRequest limitedRequest)
		{
			return await next(cancellationToken);
		}

		Debug.Assert(limitedRequest is IAuthorizedRequest, "UsageLimited request must be also AuthorizedRequest");

		var userId = userProvider.CurrentUser.Id;
		var policyName = limitedRequest.UsageLimitingPolicyName;

		var matchingPolicy =
			limitingOptions.Value.Policies.FirstOrDefault(p => p.Name == policyName);
		if (matchingPolicy is null)
		{
			throw new InvalidOperationException($"Usage Limiting Policy {policyName} was not found.");
		}

		var remainOperations = await limitingRepository.GetOrAddPolicyCount(
			userId, policyName, matchingPolicy.OperationsCount, matchingPolicy.RefreshTime);
		if (remainOperations <= 0)
		{
			return (dynamic) SecurityErrors.UsageLimited();
		}

		//todo LOCK
		var result = await next(cancellationToken);
		var resultBase = result as ResultBase ??
		                 throw new InvalidCastException($"Failed to convert the result to type ResultBase");

		if (resultBase.IsFailure)
		{
			// If the request could not be executed correctly, we do not decrease the operation counter.
			return result;
		}

		await limitingRepository.DecreasePolicyCount(userId, policyName);

		return result;
	}
}
