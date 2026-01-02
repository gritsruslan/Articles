using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Shared.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddOptionsWithValidation<TOptions>(
			this IServiceCollection services,
			IConfiguration configuration) where TOptions : class
		{
			return services.AddOptions<TOptions>()
				.Bind(configuration.GetRequiredSection(typeof(TOptions).Name))
				//.ValidateDataAnnotations() TODO
				.ValidateOnStart()
				.Services;
		}
	}
}
