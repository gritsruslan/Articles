using System.Security.Cryptography;
using System.Text;
using Articles.Application.Authentication;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Articles.Infrastructure.Authentication;

internal sealed class RefreshTokenManager(
	IOptions<RefreshTokenOptions> options,
	ILogger<RefreshTokenManager> logger,
	ISymmetricCryptoService cryptoService) : IRefreshTokenManager
{
	private readonly byte[] _key = Encoding.UTF8.GetBytes(options.Value.Key);

	public Task<string> CreateEncrypted(
		UserId userId,
		SessionId sessionId,
		CancellationToken cancellationToken)
	{
		var refreshToken = new RefreshToken
		{
			UserId = userId,
			SessionId = sessionId,
			Audience = options.Value.Audience,
			Issuer = options.Value.Issuer
		};

		var json = JsonConvert.SerializeObject(refreshToken);
		return cryptoService.EncryptAsync(json, _key, cancellationToken);
	}

	public Result Validate(RefreshToken refreshToken)
	{
		if (refreshToken.Issuer != options.Value.Issuer)
		{
			return SecurityErrors.Unauthorized();
		}
		if (refreshToken.Audience != options.Value.Audience)
		{
			return SecurityErrors.Unauthorized();
		}

		return Result.Success();
	}

	public async Task<Result<RefreshToken>> Decrypt(string? refreshToken, CancellationToken cancellationToken)
	{
		if (refreshToken is null)
		{
			return SecurityErrors.Unauthorized();
		}

		try
		{
			string json = await cryptoService.DecryptAsync(refreshToken, _key, cancellationToken);
			RefreshToken? token = JsonConvert.DeserializeObject<RefreshToken>(json);

			if (token is null)
			{
				logger.LogWarning(
					"Failed to deserialize json refresh token into RefreshToken type, " +
					"the json is {RefreshTokenJson}", json);
				return SecurityErrors.InvalidRefreshToken();
			}

			return token;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Failed to decrypt refresh token, maybe someone is " +
			                      "trying to forge it. Encrypted value is {encryptedRefreshToken}", refreshToken);
			return SecurityErrors.InvalidRefreshToken();
		}
	}
}
