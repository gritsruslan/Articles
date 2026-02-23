using System.Threading.RateLimiting;
using Articles.Application.Interfaces.Authentication;
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

	public const string AuthRateLimitingPolicy = "Auth";

	public const string ApiRateLimitingPolicy = "Api";

	public static IServiceCollection AddRateLimiting(this IServiceCollection services)
	{
		return services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
				RateLimitPartition.GetFixedWindowLimiter(
					"global",
					_ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 1000,
						Window = TimeSpan.FromSeconds(1)
				}));

			options.AddPolicy(AuthRateLimitingPolicy, httpContext =>
			{
				var ip = httpContext.Connection.RemoteIpAddress?.ToString();
				if (ip is null)
				{
					return RateLimitPartition.GetNoLimiter("no-ip");
				}

				return RateLimitPartition.GetFixedWindowLimiter(ip, _ =>
					new FixedWindowRateLimiterOptions
					{
						PermitLimit = 20,
						Window = TimeSpan.FromSeconds(10),
						QueueLimit = 10
					});
			});

			options.AddPolicy(ApiRateLimitingPolicy, httpContext =>
			{
				var userProvider = httpContext.RequestServices.GetRequiredService<IApplicationUserProvider>();

				if (userProvider.CurrentUser.IsGuest)
				{
					return RateLimitPartition.GetNoLimiter("Guest");
				}

				var userId = userProvider.CurrentUser.Id.Value.ToString();
				return RateLimitPartition.GetTokenBucketLimiter(userId, _ =>
					new TokenBucketRateLimiterOptions
					{
						TokenLimit = 100,
						ReplenishmentPeriod = TimeSpan.FromSeconds(10),
						TokensPerPeriod = 50,
						AutoReplenishment = true
					});
			});
		});
	}

	public const string DefaultCorsPolicy = "Default";

	public static IServiceCollection ConfigureCors(this IServiceCollection services,
		IConfiguration configuration)
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
