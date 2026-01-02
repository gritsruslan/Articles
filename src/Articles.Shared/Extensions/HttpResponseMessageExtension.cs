namespace Articles.Shared.Extensions;

public static class HttpResponseMessageExtension
{
	public static bool HasMeaningfulCookie(this HttpResponseMessage response, string cookieName)
	{
		if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
		{
			// no cookies at all
			return false;
		}

		var cookie = cookies.FirstOrDefault(c => c.StartsWith(cookieName));
		if (cookie is null)
		{
			return false;
		}

		var value = cookie.Split("=", 2)[1].Split(";")[0];
		return !string.IsNullOrWhiteSpace(value);
	}

	public static string? GetCookie(this HttpResponseMessage response, string cookieName)
	{
		var cookie = response.Headers.GetValues("Set-Cookie")
			.FirstOrDefault(c => c.StartsWith(cookieName));
		return cookie?.Split("=", 2)[1].Split(";")[0];
	}
}
