using Microsoft.Extensions.Configuration;

namespace Articles.Shared.Extensions;

public static class ConfigurationExtensions
{
	public static string GetRequiredConnectionString(this IConfiguration configuration, string key)
	{
		return configuration.GetConnectionString(key) ??
		       throw new ArgumentException($"Connection string \"{key}\" is not defined.");
	}
}
