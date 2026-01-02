using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Extensions;
using Articles.Storage.Redis.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Articles.Storage.Redis;

public static class DependencyInjection
{
	public static IServiceCollection AddRedis(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddSingleton<IConnectionMultiplexer>(_ =>
			ConnectionMultiplexer.Connect(configuration.GetRequiredConnectionString("Redis")));
		services.AddScoped<IDatabase>(sp =>
			sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

		services.AddScoped<IUsageLimitingRepository, UsageLimitingRepository>();

		return services;
	}
}
