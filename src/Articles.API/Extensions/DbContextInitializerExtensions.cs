using Articles.Storage.Postgres.Initializer;

namespace Articles.API.Extensions;

internal static class DbContextInitializerExtensions
{
	public static async Task InitializeDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var initializer = scope.ServiceProvider.GetRequiredService<IDbContextInitializer>();

		await initializer.MigrateAsync();
		await initializer.SeedAsync();
	}
}
