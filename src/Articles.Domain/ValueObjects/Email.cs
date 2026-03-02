using System.Text.RegularExpressions;
using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public partial record struct Email
{
	private Email(string email) => Value = email;

	public string Value { get; }

	public static Result<Email> Create(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return UserErrors.EmptyEmail();
		}

		if (email.Length is < UserConstants.EmailMinLength or > UserConstants.EmailMaxLength)
		{
			return UserErrors.InvalidEmailLength(email);
		}

		if (!EmailRegex().IsMatch(email))
		{
			return UserErrors.InvalidEmail(email);
		}

		return new Email(email);
	}

	// Use only in cases where you are sure that the email is valid
	public static Email CreateVerified(string emailStr) => new(emailStr);

	[GeneratedRegex(UserConstants.EmailRegex)]
	private static partial Regex EmailRegex();
}
