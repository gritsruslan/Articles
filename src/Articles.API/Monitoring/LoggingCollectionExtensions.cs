using Serilog;

namespace Articles.API.Monitoring;

internal static class LoggingCollectionExtensions
{
	public static IServiceCollection AddApiLogging(
		this IServiceCollection services,
		IWebHostEnvironment environment)
	{
		services.AddLogging(lb => lb.AddSerilog(
			new LoggerConfiguration()
				.MinimumLevel.Information()
				.Enrich.WithProperty("Application", "Articles.API")
				.Enrich.WithProperty("Environment", environment.EnvironmentName)
				.WriteTo.Console()
				.CreateLogger()
		));

		return services;
	}
}
