using Articles.Shared.Result;
using MediatR;

namespace Articles.Shared.CQRS;

public interface IQueryHandler<in TQuery, TResponse> :
	IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>;
