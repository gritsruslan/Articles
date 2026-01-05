using Articles.Shared.Extensions;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;

namespace Articles.API.Monitoring;

internal static class LoggingCollectionExtensions
{
	public static IServiceCollection AddApiLogging(
		this IServiceCollection services,
		IConfiguration configuration,
		IWebHostEnvironment environment)
	{
		var logLevelSwitch = new LoggingLevelSwitch(initialMinimumLevel: LogEventLevel.Warning);
		services.AddSingleton(logLevelSwitch);

		services.AddLogging(lb => lb.AddSerilog(
			new LoggerConfiguration()
				.MinimumLevel.Information()
				.Enrich.WithProperty("Application", "Articles.API")
				.Enrich.WithProperty("Environment", environment.EnvironmentName)
				.WriteTo.Console()
				.WriteTo.Logger(lc => lc.MinimumLevel.ControlledBy(logLevelSwitch)
					.Filter.ByExcluding(Matching.FromSource("Microsoft"))
					.WriteTo.GrafanaLoki(configuration.GetRequiredConnectionString("Loki"),
						propertiesAsLabels: ["Application", "Environment"])
				)
				.CreateLogger()
		));

		return services;
	}
}
