using System.Text.Json.Serialization;

namespace Articles.Shared.Result;

public sealed class Error(
	ErrorType type,
	string? message = null,
	string? code = null,
	string? invalidObject = null,
	string? invalidObjectValueString = null)
{
	[JsonIgnore]
	public ErrorType Type { get; } = type;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Message { get; } = message;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string Code => code ?? ConvertErrorTypeToCode(Type);

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? InvalidObject { get; } = invalidObject;

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? InvalidObjectValue { get; } = invalidObjectValueString;

	[JsonIgnore] public bool IsOnlyErrorCode => Message is null && InvalidObject is null;

	private static string ConvertErrorTypeToCode(ErrorType errorType)
	{
		return errorType switch
		{
			ErrorType.Validation or ErrorType.InvalidValue => "object.is.invalid",
			ErrorType.NotFound => "object.not.found",
			ErrorType.Failure => "operation.failure",
			ErrorType.Conflict => "operation.conflict",
			ErrorType.Internal => "internal.server.error",
			ErrorType.Unauthorized => "unauthorized",
			ErrorType.Forbidden => "forbidden",
			_ => "error"
		};
	}
}
