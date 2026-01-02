namespace Articles.API.Authentication;

internal interface IAuthTokenStorage
{
	void StoreAccessToken(string accessToken);

	void StoreRefreshToken(string refreshToken);

	string? GetRefreshToken();

	string? GetAccessToken();

	string GetUserAgent();

	void RemoveAccessToken();

	void RemoveRefreshToken();
}
