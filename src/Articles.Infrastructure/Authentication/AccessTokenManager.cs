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
	ISymmetricCryptoService cryptoService,
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
		return cryptoService.EncryptAsync(json, _key, cancellationToken);
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

	public async Task<Result<AccessToken>> Decrypt(string accessToken, CancellationToken cancellationToken)
	{
		try
		{
			string json = await cryptoService.DecryptAsync(accessToken, _key, cancellationToken);
			AccessToken? token = JsonConvert.DeserializeObject<AccessToken>(json);

			if (token is null)
			{
				logger.LogWarning(
					"Failed to deserialize json access token into AccessToken type, " +
					"the json is {accessTokenJson}", json);
				return SecurityErrors.InvalidAccessToken();
			}

			return token;
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Corrupted decrypt access token, maybe someone is trying to forge it. " +
			                  "Encrypted value is {encryptedAccessToken}. ", accessToken);
			return SecurityErrors.InvalidAccessToken();
		}
	}
}
