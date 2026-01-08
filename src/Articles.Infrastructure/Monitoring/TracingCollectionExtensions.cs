using System.Diagnostics;
using Articles.Domain.Constants;
using Articles.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Articles.Infrastructure.Monitoring;

public static class TracingCollectionExtensions
{
	public static IServiceCollection AddApiTracing(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddOpenTelemetry()
			.WithTracing(b => b
			.SetResourceBuilder(
				ResourceBuilder.CreateDefault().AddService(OverallConstants.DomainName)
			)
			.AddEntityFrameworkCoreInstrumentation()
			.AddRedisInstrumentation()
			.AddAspNetCoreInstrumentation(options =>
				{
					options.Filter += Filter;
					options.EnrichWithHttpResponse = EnrichWithHttpResponse;
				}
			)
			.AddSource(OverallConstants.DomainName)
			.AddOtlpExporter(tpb => tpb.Endpoint =
				new Uri(configuration.GetRequiredConnectionString("Jaeger")))
		);

		return services;
	}

	private static bool Filter(HttpContext httpContext)
	{
		return !httpContext.Request.Path.StartsWithSegments("/metrics") &&
		       !httpContext.Request.Path.StartsWithSegments("/swagger") &&
		       !httpContext.Request.Path.StartsWithSegments("/health");
	}

	private static void EnrichWithHttpResponse(Activity activity, HttpResponse response)
	{
		string result = response.StatusCode switch
		{
			< 400 => "success",
			< 500 => "failure",
			_ => "error"
		};
		activity.AddTag(nameof(result), result);
	}
}
