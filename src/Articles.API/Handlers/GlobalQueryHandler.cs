using Articles.API.Extensions;
using Articles.Shared.CQRS;
using Articles.Shared.Result;
using MediatR;

namespace Articles.API.Handlers;

internal sealed class GlobalQueryHandler(ISender sender)
{
	public async Task<IResult> Handle<TResult>(
		IQuery<TResult> query,
		Func<TResult, IResult> onSuccess,
		CancellationToken cancellationToken = default)
	{
		Result<TResult> result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return onSuccess(result.Value);
	}
}
