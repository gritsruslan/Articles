using System.Text.RegularExpressions;
using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public partial record struct Email
{
	private Email(string emailStr) => Value = emailStr;

	public string Value { get; }

	public static Result<Email> Create(string emailStr)
	{
		if (string.IsNullOrWhiteSpace(emailStr))
		{
			return UserErrors.EmptyEmail();
		}

		if (emailStr.Length is < UserConstants.EmailMinLength or > UserConstants.EmailMaxLength)
		{
			return UserErrors.InvalidEmailLength(emailStr);
		}

		if (!EmailRegex().IsMatch(emailStr))
		{
			return UserErrors.InvalidEmail(emailStr);
		}

		return new Email(emailStr);
	}

	// Use only in cases where you are sure that the email is valid
	public static Email CreateVerified(string emailStr) => new(emailStr);

	[GeneratedRegex(UserConstants.EmailRegex)]
	private static partial Regex EmailRegex();
}
