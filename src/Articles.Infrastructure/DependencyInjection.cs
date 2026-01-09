using System.Net.Mail;
using Articles.Application.Authorization;
using Articles.Application.Interfaces.Mail;
using Articles.Application.Interfaces.Monitoring;
using Articles.Infrastructure.Authentication;
using Articles.Infrastructure.Authorization;
using Articles.Infrastructure.Mail;
using Articles.Infrastructure.Monitoring;
using Articles.Infrastructure.Security;
using Articles.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddSingleton<IPasswordHasher, PasswordHasher>()
			.AddSingleton<IPasswordValidator, PasswordValidator>();

		services.AddScoped<IAccessTokenManager, AccessTokenManager>()
			.AddScoped<IRefreshTokenManager, RefreshTokenManager>()
			.AddSingleton<IEmailConfirmationTokenManager, EmailConfirmationTokenManager>();

		services.AddSingleton<ISymmetricCryptoService, AesSymmetricCryptoService>();

		services.AddScoped<IApplicationUserProvider, ApplicationUserProvider>()
			.AddScoped<IAuthenticationService, AuthenticationService>()
			.AddScoped<IAuthorizationService, AuthorizationService>()
			.AddSingleton<ISessionManager, SessionManager>();

		var options = configuration.GetRequiredSection(nameof(RolesOptions)).Get<RolesOptions>()!;
		IRoleManager roleManager = RoleManager.ParseFromOptions(options);
		services.AddSingleton(roleManager);

		/*
		services.AddSingleton<IRoleManager>(_ =>
		{
			var options = configuration.GetRequiredSection(nameof(RolesOptions)).Get<RolesOptions>()!;
			return RoleManager.ParseFromOptions(options);
		});
		*/

		services.AddTransient<IMailSender, MailSender>(static serviceProvider =>
		{
			var smtpOptions = serviceProvider.GetRequiredService<IOptions<SmtpClientOptions>>().Value;
			var mailingOptions = serviceProvider.GetRequiredService<IOptions<MailingOptions>>();
			var smtpClient = new SmtpClient(smtpOptions.Host, smtpOptions.Port);
			return new MailSender(mailingOptions, smtpClient);
		});

		//monitoring
		services.AddSingleton<IUseCaseMetricsService, UseCaseMetricsService>()
			.AddSingleton<IOutboxMetricsService, OutboxMetricsService>();

		services.AddSingleton<IUseCaseTracingSource, UseCaseTracing>();
		services.AddSingleton<IOutboxTracingSource, OutboxTracing>();

		return services;
	}
}
