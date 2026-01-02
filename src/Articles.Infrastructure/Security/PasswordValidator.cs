using System.Text.RegularExpressions;
using Articles.Application.Interfaces.Security;
using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Infrastructure.Security;

internal sealed partial class PasswordValidator : IPasswordValidator
{
	public Result Validate(string password)
	{
		if (string.IsNullOrEmpty(password))
		{
			return SecurityErrors.PasswordIsEmpty();
		}

		if (password.Length < SecurityConstants.PasswordMinLength)
		{
			return SecurityErrors.PasswordIsTooShort(SecurityConstants.PasswordMinLength, password.Length);
		}

		if (password.Length > SecurityConstants.PasswordMaxLength)
		{
			return SecurityErrors.PasswordIsTooLong(SecurityConstants.PasswordMaxLength, password.Length);
		}

		if (!UpperCaseLetterRegex().IsMatch(password))
		{
			return SecurityErrors.PasswordMustContainUpperLetter();
		}

		if (!LowerCaseLetterRegex().IsMatch(password))
		{
			return SecurityErrors.PasswordMustContainLowerLetter();
		}

		if (!DigitRegex().IsMatch(password))
		{
			return SecurityErrors.PasswordMustContainDigit();
		}

		return Result.Success();
	}

	// Contains at least one upper case letter
	[GeneratedRegex(SecurityConstants.UpperCaseLetterRegex)]
	private static partial Regex UpperCaseLetterRegex();

	// Contains at least one lower case letter
	[GeneratedRegex(SecurityConstants.LowerCaseLetterRegex)]
	private static partial Regex LowerCaseLetterRegex();

	// Contains at least one digit
	[GeneratedRegex(SecurityConstants.DigitRegex)]
	private static partial Regex DigitRegex();
}
