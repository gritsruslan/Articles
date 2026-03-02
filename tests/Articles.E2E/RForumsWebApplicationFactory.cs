using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Articles.E2E;

public class ArticlesWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder().Build();

	private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var newConfiguration = new ConfigurationBuilder().AddInMemoryCollection(
			new Dictionary<string, string>
			{
			["ConnectionStrings:Postgres"] = _postgresSqlContainer.GetConnectionString(),
			["ConnectionStrings:Redis"] = _redisContainer.GetConnectionString(),
			["ASPNETCORE_ENVIRONMENT"] = "Development",
			["EmailConfirmationTokenOptions:Key"] =  "abcdabcdabcd1234",
			["AccessTokenOptions:Key"] =  "VCwT8JrSfUPIql5qwOSGPLgQnWrTwULv",
			["RefreshTokenOptions:Key"] =  "0BeUpZGAEBxWrXLOLEUTtgVgF5diWpYF",
			["SupervisorUserOptions:Password"] =  "admin",
			["MinioOptions:AccessKey"] =  "admin",
			["MinioOptions:SecretKey"] =  "admin123"
		}!).Build();

		builder.UseConfiguration(newConfiguration);
		base.ConfigureWebHost(builder);
	}

	public async Task InitializeAsync()
	{
		await _postgresSqlContainer.StartAsync();
		await _redisContainer.StartAsync();
	}

	public new async Task DisposeAsync()
	{
		await _postgresSqlContainer.DisposeAsync();
		await _redisContainer.DisposeAsync();
	}
}
