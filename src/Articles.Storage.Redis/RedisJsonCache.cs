using Newtonsoft.Json;
using StackExchange.Redis;

namespace Articles.Storage.Redis;

// helper class for redis caching
internal sealed class RedisJsonCache(IDatabase database)
{
	public Task<T> CacheAsJson<T>(
		RedisKey key,
		Func<Task<T>> factory,
		TimeSpan? ttl = null) =>
		CacheAsJsonWithCondition(key, true, factory, ttl);

	public async Task<T> CacheAsJsonWithCondition<T>(
		RedisKey key,
		bool cacheCondition,
		Func<Task<T>> factory,
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
