using Articles.Application.Authentication;

namespace Articles.Application.Interfaces.Security;

public interface IEmailConfirmationTokenManager
{
	Task<string> EncryptToken(EmailConfirmationToken token, CancellationToken cancellationToken);

	Result Validate(EmailConfirmationToken token);

	Task<Result<EmailConfirmationToken>> DecryptToken(string encryptedToken, CancellationToken cancellationToken);
}
