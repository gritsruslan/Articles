using Articles.Shared.Result;
using MediatR;

namespace Articles.Shared.CQRS;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
