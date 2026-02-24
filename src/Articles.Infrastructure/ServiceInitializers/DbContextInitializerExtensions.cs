using Articles.Storage.Postgres.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Infrastructure.ServiceInitializers;

public static class DbContextInitializerExtensions
{
	public static async Task InitializeDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var initializer = scope.ServiceProvider.GetRequiredService<IDbContextInitializer>();

		await initializer.MigrateAsync();
		await initializer.SeedAsync();
	}
}
