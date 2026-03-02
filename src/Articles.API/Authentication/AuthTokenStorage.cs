using Articles.API.Constants;

namespace Articles.API.Authentication;

internal sealed class AuthTokenStorage(
	IHttpContextAccessor httpContextAccessor,
	IWebHostEnvironment environment) : IAuthTokenStorage
{
	private HttpContext HttpContext => httpContextAccessor.HttpContext!;

	public void StoreAccessToken(string accessToken)
	{
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = false,
			SameSite = SameSiteMode.Strict
		};

		if (environment.IsProduction())
		{
			cookieOptions.Secure = true;
		}

		HttpContext.Response.Cookies.Append(AuthTokenHeaders.AccessToken, accessToken, cookieOptions);
	}

	public void StoreRefreshToken(string refreshToken)
	{
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = false,
			SameSite = SameSiteMode.Strict,
		};

		if (environment.IsProduction())
		{
			cookieOptions.Secure = true;
			cookieOptions.Path = "/auth/refresh";
		}

		HttpContext.Response.Cookies.Append(AuthTokenHeaders.RefreshToken, refreshToken, cookieOptions);
	}

	public string? GetRefreshToken() => HttpContext.Request.Cookies[AuthTokenHeaders.RefreshToken];

	public string? GetAccessToken() => HttpContext.Request.Cookies[AuthTokenHeaders.AccessToken];

	public string GetUserAgent() => HttpContext.Request.Headers.UserAgent.ToString();

	public void RemoveAccessToken() => HttpContext.Response.Cookies.Delete(AuthTokenHeaders.AccessToken);

	public void RemoveRefreshToken() => HttpContext.Response.Cookies.Delete(AuthTokenHeaders.RefreshToken);
}
