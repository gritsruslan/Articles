using Articles.Domain.ValueObjects;

namespace Articles.Application.Interfaces.Mail;

public interface IMailSender
{
	Task SendEmail(Email recipientEmail, string subject, string body, CancellationToken cancellationToken);
}
