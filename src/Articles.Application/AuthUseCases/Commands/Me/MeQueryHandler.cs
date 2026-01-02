using Articles.Application.Authentication;
using Articles.Application.Interfaces.Authentication;
using Articles.Shared.CQRS;
using Articles.Shared.Result;

namespace Articles.Application.AuthUseCases.Commands.Me;

internal sealed class MeQueryHandler(IApplicationUserProvider userProvider) : IQueryHandler<MeQuery, RecognizedUser>
{
	public Task<Result<RecognizedUser>> Handle(MeQuery request, CancellationToken cancellationToken)
	{
		return Task.FromResult(Result<RecognizedUser>.Success(userProvider.CurrentUser));
	}
}
