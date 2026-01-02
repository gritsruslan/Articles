using System.Net.Mail;
using Articles.Application.Interfaces.Mail;
using Articles.Shared.Options;

namespace Articles.Infrastructure.Mail;

internal sealed class MailSender(
	IOptions<MailingOptions> mailingOptions,
	SmtpClient smtpClient) : IMailSender
{
	public async Task SendEmail(
		Email recipientEmail, string subject, string body, CancellationToken cancellationToken)
	{
		await smtpClient.SendMailAsync(new MailMessage(
				mailingOptions.Value.HostEmail,
				recipientEmail.Value,
				subject,
				body
			),
			cancellationToken);
	}
}
