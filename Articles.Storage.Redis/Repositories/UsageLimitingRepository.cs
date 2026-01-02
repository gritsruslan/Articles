using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Identifiers;
using StackExchange.Redis;

namespace Articles.Storage.Redis.Repositories;

internal sealed class UsageLimitingRepository(IDatabase database) : IUsageLimitingRepository
{
	private RedisKey GenerateKey(string policyName, UserId userId) =>
		(RedisKey) $"rforums.usage-limiting-policy.{policyName}.{userId.Value}";

	public async Task<int> GetOrAddPolicyCount(
		UserId userId,
		string policyName,
		int initialCount,
		TimeSpan refreshTime)
	{
		var key = GenerateKey(policyName, userId);
		var value = (RedisValue) initialCount;
		var policy = await database.StringGetWithExpiryAsync(key);

		if (!policy.Value.HasValue)
		{
			await database.StringSetAsync(key, value, refreshTime);
			return initialCount;
		}

		if (!int.TryParse(policy.Value.ToString(), out var operationsLeft))
		{
			throw new InvalidOperationException("Invalid usage limit number in hash");
		}

		return operationsLeft;
	}

	public async Task DecreasePolicyCount(UserId userId, string policyName) =>
		await database.StringDecrementAsync(GenerateKey(policyName, userId));
}
