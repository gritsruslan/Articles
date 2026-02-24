using Articles.API.Constants;
using Articles.Shared.Options;

namespace Articles.API.Extensions;

internal static class CorsServiceCollectionExtensions
{
	public static IServiceCollection ConfigureCors(this IServiceCollection services,
		IConfiguration configuration)
	{
		return services.AddCors(options =>
			options.AddPolicy(CorsConstants.DefaultPolicy, policyBuilder =>
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
