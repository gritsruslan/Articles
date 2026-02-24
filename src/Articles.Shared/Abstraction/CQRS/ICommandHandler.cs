using Articles.Shared.Result;
using MediatR;

namespace Articles.Shared.Abstraction.CQRS;

public interface ICommandHandler<in TCommand> :
	IRequestHandler<TCommand, Result.Result> where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> :
	IRequestHandler<TCommand, Result<TResponse>> where TCommand : ICommand<TResponse>;
