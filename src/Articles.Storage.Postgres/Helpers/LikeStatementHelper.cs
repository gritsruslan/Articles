namespace Articles.Storage.Postgres.Helpers;

public static class LikeStatementHelper
{
	public const string EscapeCharacter = @"\";

	public static string Normalize(string inputQuery)
	{
		return inputQuery.Trim()
			.Replace(@"\", @"\\")
			.Replace("%", @"\%")
			.Replace("_", @"\_");
	}
}
