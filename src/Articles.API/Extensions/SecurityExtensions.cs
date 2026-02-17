using System.Threading.RateLimiting;
using Articles.Shared.Options;
using Microsoft.AspNetCore.Http.Features;

namespace Articles.API.Extensions;

public static class SecurityExtensions
{
	public const long MaxRequestBodySize = 128_000_000;

	public static WebApplicationBuilder ConfigureMaxRequestBodySize(
		this WebApplicationBuilder builder)
	{
		builder.WebHost.ConfigureKestrel(options =>
			options.Limits.MaxRequestBodySize = MaxRequestBodySize);
		builder.Services.Configure<FormOptions>(options =>
			options.MultipartBodyLengthLimit = MaxRequestBodySize);

		return builder;
	}

	public static IServiceCollection AddRateLimiting(this IServiceCollection services)
	{
		return services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				var ipAddress = httpContext.Connection.RemoteIpAddress!.ToString();
				return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
					new FixedWindowRateLimiterOptions
					{
						PermitLimit = 10,
						Window = TimeSpan.FromSeconds(10)
					});
			});
		});
	}

	public const string DefaultCorsPolicy = "DefaultPolicy";

	public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
	{
		return services.AddCors(options =>
			options.AddPolicy(DefaultCorsPolicy, policyBuilder =>
			{
				var integrationOptions = configuration
					.GetRequiredSection(nameof(IntegrationOptions)).Get<IntegrationOptions>()!;

				policyBuilder
					.WithOrigins(integrationOptions.AllowedOrigins)
					.AllowAnyMethod()
					.AllowAnyHeader()
					.Build();
			})
		);
	}
}
