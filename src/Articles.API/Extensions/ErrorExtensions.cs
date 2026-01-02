using Articles.Shared.Result;

namespace Articles.API.Extensions;

internal static class ErrorExtensions
{
	public static IResult ToResponse(this Error error)
	{
		var statusCode = ConvertErrorTypeToStatusCode(error.Type);
		if (error.IsOnlyErrorCode)
		{
			return Results.StatusCode(statusCode);
		}

		return TypedResults.Json(error, statusCode: statusCode);
	}

	private static int ConvertErrorTypeToStatusCode(ErrorType errorType)
	{
		return errorType switch
		{
			ErrorType.Validation
				or ErrorType.InvalidValue
				or ErrorType.Failure
				=> StatusCodes.Status400BadRequest,
			ErrorType.NotFound => StatusCodes.Status404NotFound,
			ErrorType.Conflict => StatusCodes.Status409Conflict,
			ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
			ErrorType.Forbidden => StatusCodes.Status403Forbidden,
			_ => StatusCodes.Status500InternalServerError
		};
	}
}
