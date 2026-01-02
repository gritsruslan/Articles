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
	ISymmetricCryptoService encryptionService) : IRefreshTokenManager
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
		return encryptionService.EncryptAsync(json, _key, cancellationToken);
	}

	public Result Validate(RefreshToken refreshToken)
	{
		//TODO maybe log
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

	//IMPROVEMENT maybe remove async
	public async Task<Result<RefreshToken>> Decrypt(string? refreshTokenStr, CancellationToken cancellationToken)
	{
		if (refreshTokenStr is null)
		{
			return SecurityErrors.Unauthorized();
		}

		string json;
		try
		{
			json = await encryptionService.DecryptAsync(refreshTokenStr, _key, cancellationToken);
		}
		catch (CryptographicException ex)
		{
			logger.LogWarning(ex, "Failed to decrypt refresh token, maybe someone is " +
			                      "trying to forge it. Encrypted value is {EncryptedRefreshToken}", refreshTokenStr);
			return SecurityErrors.InvalidRefreshToken();
		}

		RefreshToken? refreshToken;
		try
		{
			refreshToken = JsonConvert.DeserializeObject<RefreshToken>(json);
		}
		catch (JsonException ex)
		{
			logger.LogWarning(
				"Failed to deserialize json refresh token into RefreshToken type, " +
				"the json is {RefreshTokenJson}," +
				"exception is {Exception}", json, ex);
			return SecurityErrors.InvalidRefreshToken();
		}
		if (refreshToken is null)
		{
			logger.LogWarning(
				"Failed to deserialize json refresh token into RefreshToken type, " +
				"the json is {RefreshTokenJson}", json);
			return SecurityErrors.InvalidRefreshToken();
		}

		return refreshToken;
	}
}
