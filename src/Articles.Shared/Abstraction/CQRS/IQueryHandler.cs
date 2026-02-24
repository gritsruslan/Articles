using Articles.Shared.Result;
using MediatR;

namespace Articles.Shared.Abstraction.CQRS;

public interface IQueryHandler<in TQuery, TResponse> :
	IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>;
