namespace Articles.Shared.DefaultServices;

public static class SearchPatternHelper
{
	public const string EscapeCharacter = @"\";

	public static string Normalize(string inputQuery)
	{
		return inputQuery
			.Replace(@"\", @"\\")
			.Replace("%", @"\%")
			.Replace("_", @"\_")
			.Trim()
			.ToLowerInvariant();
	}
}
