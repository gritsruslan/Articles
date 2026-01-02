using Articles.Application.Authentication;
using Articles.Application.Authorization;
using Articles.Application.UsageLimiting;
using Articles.Domain.Constants;
using Articles.Domain.Permissions;
using Articles.Shared.CQRS;

namespace Articles.Application.AuthUseCases.Commands.Me;

public sealed record MeQuery : IQuery<RecognizedUser>, IAuthorizedQuery, IUsageLimitedQuery
{
	public int RequiredPermissionId => (int)DefaultPermissions.RequireAuthorization;

	public string UsageLimitingPolicyName => UsageLimitingPolicies.ForTesting;
}
