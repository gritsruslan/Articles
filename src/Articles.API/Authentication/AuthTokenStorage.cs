using Articles.API.Constants;

namespace Articles.API.Authentication;

internal sealed class AuthTokenStorage(IHttpContextAccessor httpContextAccessor) : IAuthTokenStorage
{
	private HttpContext HttpContext => httpContextAccessor.HttpContext!;

	public void StoreAccessToken(string accessToken)
	{
		HttpContext.Response.Cookies.Append(AuthTokenHeaders.AccessToken, accessToken, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict
		});
	}

	public void StoreRefreshToken(string refreshToken)
	{
		HttpContext.Response.Cookies.Append(AuthTokenHeaders.RefreshToken, refreshToken, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Path = "/auth/refresh"
		});
	}

	public string? GetRefreshToken() => HttpContext.Request.Cookies[AuthTokenHeaders.RefreshToken];

	public string? GetAccessToken() => HttpContext.Request.Cookies[AuthTokenHeaders.AccessToken];

	public string GetUserAgent() => HttpContext.Request.Headers.UserAgent.ToString();

	public void RemoveAccessToken() => HttpContext.Response.Cookies.Delete(AuthTokenHeaders.AccessToken);

	public void RemoveRefreshToken() => HttpContext.Response.Cookies.Delete(AuthTokenHeaders.RefreshToken);
}
