using Articles.Application.Authorization;
using MediatR;

namespace Articles.Application.Behaviours;

internal sealed class AuthorizationPipelineBehaviour<TRequest, TResponse>(
	IAuthorizationService authorizationService)
	: IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		if (request is not IAuthorizedRequest authorizedRequest)
		{
			return await next(cancellationToken);
		}

		var authorizationResult = authorizationService.IsAllowed(authorizedRequest.RequiredPermissionId);
		if (authorizationResult.IsFailure)
		{
			return (dynamic)authorizationResult.Error;
		}

		return await next(cancellationToken);
	}
}
