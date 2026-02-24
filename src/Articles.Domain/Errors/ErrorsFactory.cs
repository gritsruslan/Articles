using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class ErrorsFactory
{

	public static Error ObjectNotFoundById(string objectName, string objectIdString) =>
		new(ErrorType.NotFound,
			$"{objectName} with id {objectIdString} was not found",
			$"{objectName.ToLower()}.not.found",
			"id",
			objectIdString);

	public static Error RequiredParameter(string parameterName) =>
		new(
			ErrorType.Validation,
			$"{parameterName} is required",
			$"required.{parameterName.ToLower()}",
			parameterName);

	public static Error InvalidParameterLength(
		string parameterName,
		string? parameterValue,
		int minLength,
		int maxLength)
	{
		var message = $"Invalid {parameterName} length. Min length is {minLength}, max length is {maxLength}";

		return new Error(
			ErrorType.Validation,
			message,
			$"invalid.{parameterName.ToLower()}.length",
			parameterValue);
	}

	public static Error InvalidParameter(string parameterName, string parameterValue) =>
		new(
			ErrorType.Validation,
			$"Invalid {parameterName}",
			$"invalid.{parameterName.ToLower()}",
			parameterName.ToLower(),
			parameterValue);
}
