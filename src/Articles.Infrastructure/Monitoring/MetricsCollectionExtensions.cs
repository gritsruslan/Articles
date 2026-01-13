using Articles.Domain.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Articles.Infrastructure.Monitoring;

public static class MetricsCollectionExtensions
{
	public static IServiceCollection AddApiMetrics(this IServiceCollection services, IWebHostEnvironment environment)
	{
		services.AddOpenTelemetry()
			.WithMetrics(b => b.SetResourceBuilder(
					ResourceBuilder.CreateDefault()
						.AddService(OverallConstants.ApiName)
						.AddAttributes(new Dictionary<string, object>
					{
						["environment"] = environment.EnvironmentName
					}))
				.AddMeter(OverallConstants.ApiName)
				.AddMeter(OverallConstants.Outbox)
				.AddAspNetCoreInstrumentation()
				.AddPrometheusExporter());

		return services;
	}
}
