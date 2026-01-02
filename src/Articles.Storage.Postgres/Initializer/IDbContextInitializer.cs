using System.Diagnostics.CodeAnalysis;

namespace Articles.Storage.Postgres.Initializer;

public interface IDbContextInitializer
{
	Task MigrateAsync();

	Task SeedAsync();
}
