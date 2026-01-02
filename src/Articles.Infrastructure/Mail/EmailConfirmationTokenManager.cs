using System.Text;
using Articles.Application.Authentication;
using Articles.Shared.DefaultServices;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Articles.Infrastructure.Mail;

internal sealed class EmailConfirmationTokenManager(
	ISymmetricCryptoService cryptoService,
	IOptions<EmailConfirmationTokenOptions> options,
	IDateTimeProvider dateTimeProvider,
	ILogger<EmailConfirmationTokenManager> logger) : IEmailConfirmationTokenManager
{
	private readonly byte[] _key = Encoding.UTF8.GetBytes(options.Value.Key);

	public Task<string> EncryptToken(
		EmailConfirmationToken token,
		CancellationToken cancellationToken)
	{
		var json = JsonConvert.SerializeObject(token);
		return cryptoService.EncryptAsync(json, _key, cancellationToken);
	}

	public Result Validate(EmailConfirmationToken token)
	{
		if (token.ExpiresAt < dateTimeProvider.UtcNow)
		{
			return SecurityErrors.EmailConfirmationTokenExpired();
		}
		return Result.Success();
	}

	public async Task<Result<EmailConfirmationToken>> DecryptToken(
		string encryptedToken,
		CancellationToken cancellationToken)
	{
		string json;
		try
		{
			json = await cryptoService.DecryptAsync(encryptedToken, _key, cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to decrypt email confirmation token, maybe someone is " +
			                      "trying to forge it. Encrypted value is {EncryptedEmailConfirmationToken}",
				encryptedToken);
			return SecurityErrors.InvalidEmailConfirmationToken();
		}

		EmailConfirmationToken? token;
		try
		{
			token = JsonConvert.DeserializeObject<EmailConfirmationToken>(json);
		}
		catch (Exception ex)
		{
			logger.LogWarning(
				"Failed to deserialize json email confirmation token into EmailConfirmationToken type, " +
				"the json is {EmailConfirmationTokenJson}," +
				"exceptions is {Exception}", json, ex);
			return SecurityErrors.InvalidEmailConfirmationToken();
		}
		if (token is null)
		{
			logger.LogWarning(
				"Failed to deserialize json email confirmation token into EmailConfirmationToken type, " +
				"the json is {EmailConfirmationTokenJson}", json);
			return SecurityErrors.InvalidEmailConfirmationToken();
		}

		return token;
	}
}
