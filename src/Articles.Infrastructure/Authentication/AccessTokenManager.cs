using System.Security.Cryptography;
using System.Text;
using Articles.Application.Authentication;
using Articles.Shared.DefaultServices;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Articles.Infrastructure.Authentication;

internal sealed class AccessTokenManager(
	IOptions<AccessTokenOptions> options,
	ILogger<AccessTokenManager> logger,
	ISymmetricCryptoService encryptionService,
	IDateTimeProvider dateTimeProvider) : IAccessTokenManager
{
	private readonly byte[] _key = Encoding.UTF8.GetBytes(options.Value.Key);

	public Task<string> CreateEncrypted(UserId userId, CancellationToken cancellationToken)
	{
		var lifeSpan = options.Value.LifeSpan;

		var accessToken = new AccessToken
		{
			UserId = userId,
			IssuedAt = dateTimeProvider.UtcNow,
			ExpiresAt = dateTimeProvider.UtcNow.Add(lifeSpan),
			Audience = options.Value.Audience,
			Issuer = options.Value.Issuer
		};

		var json = JsonConvert.SerializeObject(accessToken);
		return encryptionService.EncryptAsync(json, _key, cancellationToken);
	}

	public Result Validate(AccessToken accessToken)
	{
		if(accessToken.ExpiresAt < dateTimeProvider.UtcNow)
		{
			return SecurityErrors.Unauthorized();
		}
		if (accessToken.Issuer != options.Value.Issuer)
		{
			return SecurityErrors.Unauthorized();
		}
		if (accessToken.Audience != options.Value.Audience)
		{
			return SecurityErrors.Unauthorized();
		}
		return Result.Success();
	}

	public async Task<Result<AccessToken>> Decrypt(string accessTokenStr, CancellationToken cancellationToken)
	{
		string json;
		try
		{
			json = await encryptionService.DecryptAsync(accessTokenStr, _key, cancellationToken);
		}
		catch (CryptographicException ex)
		{
			logger.LogWarning(ex, "Failed to decrypt access token, maybe someone is " +
			                      "trying to forge it. Encrypted value is {AccessToken}", accessTokenStr);
			return SecurityErrors.InvalidAccessToken();
		}

		AccessToken? token;
		try
		{
			token = JsonConvert.DeserializeObject<AccessToken>(json);
		}
		catch (Exception ex)
		{
			logger.LogWarning(
				"Failed to deserialize json access token into AccessToken type, " +
				"the json is {AccessTokenJson}," +
				"exceptions is {Exception}", json, ex);
			return SecurityErrors.InvalidAccessToken();
		}
		if (token is null)
		{
			logger.LogWarning(
				"Failed to deserialize json access token into AccessToken type, " +
				"the json is {AccessTokenJson}", json);
			return SecurityErrors.InvalidAccessToken();
		}

		return token;
	}
}
