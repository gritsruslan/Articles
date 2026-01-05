using Testcontainers.PostgreSql;

namespace Articles.Storage.Postgres.Tests;

public class StorageTestsFixture : IAsyncLifetime
{
	private readonly PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder().Build();

	public ArticlesDbContext GetDbContext() => new(
		new DbContextOptionsBuilder<ArticlesDbContext>()
			.UseNpgsql(_postgresSqlContainer.GetConnectionString()).Options
		);

	public virtual async Task InitializeAsync()
	{
		await _postgresSqlContainer.StartAsync();
		await GetDbContext().Database.MigrateAsync();
	}

	public async Task DisposeAsync() => await _postgresSqlContainer.DisposeAsync();
}
