namespace Articles.Domain.Constants;

public static class SecurityConstants
{
	public const int SaltLength = 32;

	public const int PasswordHashLength = 64;

	public const int PasswordMinLength = 8;

	public const int PasswordMaxLength = 32;

	public const string UpperCaseLetterRegex = "(?=.*[A-Z])";

	public const string LowerCaseLetterRegex = "(?=.*[a-z])";

	public const string DigitRegex = "(?=.*\\d)";

	public const int UserAgentMaxLength = 512;
}
