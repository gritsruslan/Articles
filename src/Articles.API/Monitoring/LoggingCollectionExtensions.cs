using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;

namespace Articles.API.Monitoring;

internal static class LoggingCollectionExtensions
{
	public static IServiceCollection AddApiLogging(
		this IServiceCollection services,
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
					// write to LOKI
				)
				.CreateLogger()
		));

		return services;
	}
}
