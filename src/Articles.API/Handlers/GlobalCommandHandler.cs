using Articles.API.Extensions;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Result;
using MediatR;

namespace Articles.API.Handlers;

internal sealed class GlobalCommandHandler(ISender sender)
{
	public async Task<IResult> Handle<TResult>(
		ICommand<TResult> command,
		Func<TResult, IResult> onSuccess,
		CancellationToken cancellationToken = default)
	{
		Result<TResult> result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return onSuccess(result.Value);
	}

	public async Task<IResult> Handle(
		ICommand command,
		Func<object?, IResult> onSuccess,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(command, cancellationToken);

		if(result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return onSuccess(null); // returns nothing
	}

	public async Task<IResult> Handle(
		ICommand command,
		Func<IResult> onSuccess,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(command, cancellationToken);

		if(result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return onSuccess(); // returns nothing
	}
}
