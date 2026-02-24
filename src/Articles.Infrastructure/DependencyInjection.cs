using System.Net.Mail;
using Articles.Application.Authorization;
using Articles.Application.Interfaces.Mail;
using Articles.Application.Interfaces.Monitoring;
using Articles.Infrastructure.Authentication;
using Articles.Infrastructure.Authorization;
using Articles.Infrastructure.Health;
using Articles.Infrastructure.Mail;
using Articles.Infrastructure.Monitoring;
using Articles.Infrastructure.Security;
using Articles.Shared.Options;
using Articles.Storage.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Articles.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services
			.AddSecurityServices()
			.AddCustomAuthentication()
			.AddCustomAuthorization(configuration)
			.ConfigureMailing()
			.AddMonitoring()
			.ConfigureHealthChecks();

		return services;
	}

	private static IServiceCollection AddCustomAuthentication(
		this IServiceCollection services)
	{
		return services.AddScoped<IAccessTokenManager, AccessTokenManager>()
			.AddScoped<IRefreshTokenManager, RefreshTokenManager>()
			.AddSingleton<IEmailConfirmationTokenManager, EmailConfirmationTokenManager>();
	}

	private static IServiceCollection AddCustomAuthorization(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddScoped<IApplicationUserProvider, ApplicationUserProvider>()
			.AddScoped<IAuthenticationService, AuthenticationService>()
			.AddScoped<IAuthorizationService, AuthorizationService>()
			.AddSingleton<ISessionManager, SessionManager>();

		var options = configuration.GetRequiredSection(nameof(RolesOptions)).Get<RolesOptions>()!;
		IRoleManager roleManager = RoleManager.ParseFromOptions(options);
		services.AddSingleton(roleManager);

		return services;
	}

	private static IServiceCollection AddMonitoring(this IServiceCollection services)
	{
		services.AddSingleton<IUseCaseMetricsService, UseCaseMetricsService>()
			.AddSingleton<IOutboxMetricsService, OutboxMetricsService>();

		services.AddSingleton<IUseCaseTracingSource, UseCaseTracing>()
			.AddSingleton<IOutboxTracingSource, OutboxTracing>();

		return services;
	}

	private static IServiceCollection ConfigureMailing(this IServiceCollection services)
	{
		return services.AddTransient<IMailSender, MailSender>(static serviceProvider =>
		{
			var smtpOptions = serviceProvider.GetRequiredService<IOptions<SmtpClientOptions>>().Value;
			var mailingOptions = serviceProvider.GetRequiredService<IOptions<MailingOptions>>();
			var smtpClient = new SmtpClient(smtpOptions.Host, smtpOptions.Port);
			return new MailSender(mailingOptions, smtpClient);
		});
	}

	private static IServiceCollection AddSecurityServices(
		this IServiceCollection services)
	{
		return services
			.AddSingleton<IPasswordHasher, PasswordHasher>()
			.AddSingleton<IPasswordValidator, PasswordValidator>()
			.AddSingleton<ISymmetricCryptoService, AesSymmetricCryptoService>();
	}

	private static IServiceCollection ConfigureHealthChecks(
		this IServiceCollection services)
	{
		return services.AddHealthChecks()
			.AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>())
			.AddDbContextCheck<ArticlesDbContext>()
			.AddCheck<MinioHealthCheck>("minio")
			.AddCheck<LokiHealthCheck>("loki")
			.AddCheck<PrometheusHealthCheck>("prometheus")
			.AddCheck<JaegerHealthCheck>("jaeger")
			.Services;
	}
}
