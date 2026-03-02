using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct CommentContent
{
	private CommentContent(string commentContent) => Value = commentContent;

	public string Value { get; }


	// Use only in cases where you are sure that the userName is valid
	public static CommentContent CreateVerified(string commentContent) => new(commentContent);

	public static Result<CommentContent> Create(string commentContent)
	{
		if (string.IsNullOrWhiteSpace(commentContent))
		{
			return CommentErrors.EmptyContent();
		}

		if (commentContent.Length is < CommentConstants.ContentMinLength or > CommentConstants.ContentMaxLength)
		{
			return CommentErrors.InvalidContentLength(commentContent);
		}

		return new CommentContent(commentContent);
	}
}


