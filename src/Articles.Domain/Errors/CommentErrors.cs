using Articles.Domain.Constants;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class CommentErrors
{
	public static Error EmptyContent() =>
		new(ErrorType.InvalidValue, "Comment content cant be empty");

	public static Error InvalidContentLength(string invalidContent) =>
		AbstractErrors.InvalidParameterLength(
			"CommentContent",
			invalidContent,
			CommentConstants.ContentMinLength,
			CommentConstants.ContentMaxLength);
}
