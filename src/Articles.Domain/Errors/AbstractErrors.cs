using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class AbstractErrors
{
	public static Error EmptyParameter(string parameterName) =>
		new(
			ErrorType.InvalidValue,
			$"{parameterName} can't be empty",
			$"empty.{parameterName.ToLower()}",
			parameterName);

	public static Error InvalidParameterLength(
		string parameterName,
		string parameterValue,
		int minLength,
		int maxLength)
	{
		var message = $"Invalid {parameterName} length. Min length is {minLength}, max length is {maxLength}";

		return new Error(
			ErrorType.InvalidValue,
			message,
			$"invalid.{parameterName.ToLower()}.length",
			parameterValue);
	}

	public static Error InvalidParameter(string parameterName, string parameterValue) =>
		new(
			ErrorType.InvalidValue,
			$"Invalid {parameterName}",
			$"invalid.{parameterName.ToLower()}",
			parameterName.ToLower(),
			parameterValue);
}
