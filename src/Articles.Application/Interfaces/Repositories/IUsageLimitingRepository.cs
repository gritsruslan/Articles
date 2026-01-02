using Articles.Domain.Identifiers;

namespace Articles.Application.Interfaces.Repositories;

public interface IUsageLimitingRepository
{
	Task<int> GetOrAddPolicyCount(UserId userId, string policyName, int initialCount, TimeSpan refreshTime);

	Task DecreasePolicyCount(UserId userId, string policyName);
}
