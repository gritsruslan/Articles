namespace Articles.Application.Authentication;

public sealed class AuthTokenPair(string accessToken, string refreshToken)
{
	public string AccessToken { get; } = accessToken;

	public string RefreshToken { get; } = refreshToken;
}
