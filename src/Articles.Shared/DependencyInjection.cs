using Articles.Shared.DefaultServices;
using Articles.Shared.Extensions;
using Articles.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Shared;

public static class DependencyInjection
{

	public static IServiceCollection AddDefaultServices(
		this IServiceCollection services)
	{
		services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

		return services;
	}


	public static IServiceCollection ConfigureOptions(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddOptionsWithValidation<SupervisorUserOptions>(configuration);

		services.AddOptionsWithValidation<MailingOptions>(configuration)
			.AddOptionsWithValidation<SmtpClientOptions>(configuration);

		services.AddOptionsWithValidation<EmailConfirmationTokenOptions>(configuration)
			.AddOptionsWithValidation<AccessTokenOptions>(configuration)
			.AddOptionsWithValidation<RefreshTokenOptions>(configuration);

		services.AddOptionsWithValidation<SessionOptions>(configuration)
			.AddOptionsWithValidation<RolesOptions>(configuration);

		services.AddOptionsWithValidation<UsageLimitingOptions>(configuration);

		return services;
	}
}
