using Articles.Application.Interfaces.Mail;
using Articles.Domain.DomainEvents;
using MediatR;

namespace Articles.Application.NotificationHandlers;

public class UserRegisteredNotificationHandler(IMailSender mailSender) : INotificationHandler<UserRegisteredDomainEvent>
{
	public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
	{
		await mailSender.SendEmail(
			Email.CreateVerified(notification.Email),
			"Welcome!", $"{notification.Name}, Welcome to the Articles!",
			cancellationToken);
	}
}
