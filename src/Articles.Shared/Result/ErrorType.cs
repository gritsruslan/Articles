namespace Articles.Shared.Result;

public enum ErrorType
{
	Validation,
	NotFound,
	InvalidValue,
	EntityTooLarge,
	UnsupportedMediaType,
	Unauthorized,
	Gone,
	Forbidden,
	Failure,
	Conflict,
	Internal
}
