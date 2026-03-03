using Newtonsoft.Json;
using StackExchange.Redis;

namespace Articles.Storage.Redis;

// helper class for redis aside caching
internal sealed class RedisHelper(IDatabase database)
{
	public async Task<T> CacheAsJson<T>(
		RedisKey key,
		Func<Task<T>> factory,
		bool cacheCondition = true,
		TimeSpan? ttl = null)
	{
		if (!cacheCondition)
		{
			return await factory();
		}

		string? json = await database.StringGetAsync(key);
		if (json is not null)
		{
			return JsonConvert.DeserializeObject<T>(json)!;
		}

		var entity = await factory();

		if (entity is not null)
		{
			await database.StringSetAsync(key, JsonConvert.SerializeObject(entity), ttl);
		}

		return entity;
	}
}
