using Articles.Shared.DefaultServices;
using Articles.Shared.Extensions;
using Articles.Shared.UnitOfWork;
using Articles.Storage.Postgres.Initializer;
using Articles.Storage.Postgres.Interceptors;
using Articles.Storage.Postgres.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Storage.Postgres;

public static class DependencyInjection
{
	public static IServiceCollection AddPostgres(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddDbContextPool<ArticlesDbContext>((sp, options) =>
		{
			options.UseNpgsql(configuration.GetRequiredConnectionString("Postgres"));
			options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

			var momentProvider = sp.GetRequiredService<IDateTimeProvider>();
			options.AddInterceptors(new AuditableEntityInterceptor(momentProvider));
		});

		services.AddScoped<IDbContextInitializer, ArticlesDbContextInitializer>();

		services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

		services.AddScoped<IUserRepository, UserRepository>()
			.AddScoped<ISessionRepository, SessionRepository>()
			.AddScoped<IDomainEventRepository, DomainEventRepository>()
			.AddScoped<IFileMetadataRepository, FileMetadataRepository>();

		return services;
	}
}
