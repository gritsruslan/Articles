using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class SecurityErrors
{
	public static Error InvalidEmailConfirmationToken() =>
		new(
			ErrorType.InvalidValue,
			"Invalid email confirmation token",
			"invalid.email-confirmation-token",
			"email.confirmation.token"
		);

	public static Error EmailConfirmationTokenExpired() =>
		new(
			ErrorType.Gone,
			"Email confirmation token has expired",
			"expired.email-confirmation-token",
			"email.confirmation.token"
		);

	public static Error EmailAlreadyConfirmed() =>
		new(ErrorType.Conflict, "Email already confirmed", "email.already.confirmed");

	public static Error PasswordIsEmpty() =>
		ErrorsFactory.RequiredParameter("password");

	public static Error PasswordIsTooShort(int minLength, int currentLength) =>
		new(
			ErrorType.Validation,
			$"Password is too short. Min length is {minLength}. Current length is {currentLength}",
			"short.password",
			"password"
		);

	public static Error PasswordIsTooLong(int maxLength, int currentLength) =>
		new(
			ErrorType.Validation,
			$"Password is too long. Max length is {maxLength}. Current length is {currentLength}",
			"long.password",
			"password");

	public static Error PasswordMustContainUpperLetter() =>
		new(
			ErrorType.Validation,
			"Password must contain at least one upper case letter",
			"password.must.contain.upper.letter",
			"password"
		);

	public static Error PasswordMustContainLowerLetter() =>
		new(
			ErrorType.Validation,
			"Password must contain at least one lower case letter",
			"password.must.contain.lower.letter",
			"password"
		);

	public static Error PasswordMustContainDigit() =>
		new(
			ErrorType.Validation,
			"Password must contain at least one digit",
			"password.must.contain.digit",
			"password"
		);

	public static Error InvalidAccessToken() =>
		new(ErrorType.Unauthorized, "Invalid access token", "invalid.access-token");

	public static Error InvalidRefreshToken() =>
		new(ErrorType.Unauthorized, "Invalid refresh token", "invalid.refresh-token");

	public static Error Unauthorized() => new(ErrorType.Unauthorized);

	public static Error SessionExpired() => new(ErrorType.Unauthorized);

	public static Error Forbidden() => new(ErrorType.Forbidden);

	public static Error RefreshTokenExpired() =>
		new(ErrorType.Unauthorized,
			"Refresh token has expired",
			"expired.refresh-token");

	public static Error IncorrectEmailOrPassword() =>
		new(
			ErrorType.Unauthorized,
			"Incorrect email or password",
			"incorrect.user-credentials");


	public static Error UsageLimited() =>
		new(ErrorType.Forbidden, "This request is usage limited, try later", "usage.limited");
}
