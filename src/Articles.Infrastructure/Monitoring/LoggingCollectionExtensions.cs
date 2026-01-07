using Articles.Domain.Constants;
using Articles.Shared.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;

namespace Articles.Infrastructure.Monitoring;

public static class LoggingCollectionExtensions
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
				.Enrich.WithProperty("Application", OverallConstants.ApiName)
				.Enrich.WithProperty("Environment", environment.EnvironmentName)
				.WriteTo.Console()
				.WriteTo.Logger(lc => lc.MinimumLevel.ControlledBy(logLevelSwitch)
					.Filter.ByExcluding(Matching.FromSource("Microsoft"))
					.Enrich.With<LogsEnricher>()
					.WriteTo.GrafanaLoki(configuration.GetRequiredConnectionString("Loki"),
						propertiesAsLabels: ["Application", "Environment"])
				)
				.CreateLogger()
		));

		return services;
	}

	private sealed class LogsEnricher : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			logEvent.AddPropertyIfAbsent(
				propertyFactory.CreateProperty("timestamp", logEvent.Timestamp.UtcDateTime));
		}
	}
}
