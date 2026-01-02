using Articles.Shared.Result;
using MediatR;

namespace Articles.Shared.CQRS;

public interface ICommand : IRequest<Result.Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
