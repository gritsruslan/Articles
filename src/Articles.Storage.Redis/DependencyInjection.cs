using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Extensions;
using Articles.Storage.Redis.Decorators;
using Articles.Storage.Redis.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Scrutor;

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

		services
			.Decorate<IBlogRepository, CachingBlogRepositoryDecorator>()
			.Decorate<IArticleRepository, CachingArticleRepositoryDecorator>()
			.Decorate<ICommentRepository, CachingCommentRepositoryDecorator>();

		return services;
	}
}
