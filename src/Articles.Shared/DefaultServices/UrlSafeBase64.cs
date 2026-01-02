namespace Articles.Shared.DefaultServices;

public static class UrlSafeBase64
{
	public static string ToUrlSafeBase64(byte[] inArray)
	{
		var base64 = Convert.ToBase64String(inArray);
		return base64
			.Replace('+', '-')
			.Replace('/', '_')
			.TrimEnd('=');
	}

	public static byte[] FromUrfSafeBase64(string str)
	{
		var base64 = str.Replace('-', '+')
						.Replace('_', '/');

		var padding = 4 - str.Length % 4;
		if (padding < 4)
		{
			base64 += new string('=', padding);
		}

		return Convert.FromBase64String(base64);
	}
}
