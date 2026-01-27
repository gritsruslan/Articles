using Articles.Domain.Constants;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class CommentErrors
{
	public static Error NotFound(CommentId commentId) =>
		new(ErrorType.InvalidValue, $"Comment with id {commentId.Value} was not found");

	public static Error EmptyContent() =>
		new(ErrorType.InvalidValue, "Comment content cant be empty");

	public static Error InvalidContentLength(string invalidContent) =>
		AbstractErrors.InvalidParameterLength(
			"CommentContent",
			invalidContent,
			CommentConstants.ContentMinLength,
			CommentConstants.ContentMaxLength);

	public static Error NotAnAuthor() =>
		new(ErrorType.Forbidden, "You are not an author of this comment");
}
