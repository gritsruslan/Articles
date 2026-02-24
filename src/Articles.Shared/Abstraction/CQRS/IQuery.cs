using Articles.Shared.Result;
using MediatR;

namespace Articles.Shared.Abstraction.CQRS;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
