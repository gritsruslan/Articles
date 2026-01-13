using Articles.Application.Authentication;
using Articles.Application.Interfaces.Mail;
using Articles.Application.Interfaces.Security;
using Articles.Domain.DomainEvents;
using Articles.Shared.DefaultServices;
using Articles.Shared.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace Articles.Application.NotificationHandlers;

public class UserRegisteredNotificationHandler(
	IMailSender mailSender,
	IDateTimeProvider dateTimeProvider,
	IOptions<EmailConfirmationTokenOptions> tokenOptions,
	IEmailConfirmationTokenManager tokenManager) : INotificationHandler<UserRegisteredDomainEvent>
{
	public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
	{
		var token = new EmailConfirmationToken()
		{
			UserId = UserId.Create(notification.UserId),
			IssuedAt = dateTimeProvider.UtcNow,
			ExpiresAt = dateTimeProvider.UtcNow.Add(tokenOptions.Value.LifeSpan)
		};

		string tokenString = await tokenManager.EncryptToken(token, cancellationToken);

		await mailSender.SendEmail(
			Email.CreateVerified(notification.Email),

			"Welcome!",

			//hardcode
			$"""
			 {notification.Name}, Welcome to the Articles!
			 To confirm your email use this link :
			 http://localhost:5025/auth/confirm-email?token={tokenString}
			 """
			,
			cancellationToken);
	}
}
