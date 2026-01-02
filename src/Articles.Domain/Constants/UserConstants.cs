namespace Articles.Domain.Constants;

public static class UserConstants
{
	public const int EmailMinLength = 5;

	public const int EmailMaxLength = 320;

	public const int NameMinLength = 1;

	public const int NameMaxLength = 50;

	public const int DomainIdMinLength = 4;

	public const int DomainIdMaxLength = 15;

	// Only letters a-z A-Z, digits and underscore '_'
	// Digit can't be first
	public const string DomainIdRegex = "^[A-Za-z_][A-Za-z0-9_]*$";

	// From https://stackoverflow.com/questions/5342375/regex-email-validation
	public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
}
