using System.Threading.RateLimiting;
using Articles.API.Constants;
using Articles.Application.Interfaces.Authentication;

namespace Articles.API.Extensions;

internal static class RateLimitingServiceCollectionExtensions
{
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

			options.AddPolicy(RateLimitingConstants.AuthPolicy, httpContext =>
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

			options.AddPolicy(RateLimitingConstants.ApiPolicy, httpContext =>
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

}
