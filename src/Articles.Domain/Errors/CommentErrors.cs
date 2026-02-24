using Articles.Domain.Constants;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class CommentErrors
{
	public static Error NotFound(CommentId commentId) =>
		ErrorsFactory.ObjectNotFoundById("comment", commentId.Value.ToString());

	public static Error EmptyContent() =>
		ErrorsFactory.RequiredParameter("comment-content");

	public static Error InvalidContentLength(string invalidContent) =>
		ErrorsFactory.InvalidParameterLength(
			"comment-content",
			invalidContent,
			CommentConstants.ContentMinLength,
			CommentConstants.ContentMaxLength);

	public static Error NotAnAuthor() =>
		new(ErrorType.Forbidden,
			"You are not an author of this comment",
			"not.an.author");
}
