using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct CommentContent
{
	private CommentContent(string commentContentStr) => Value = commentContentStr;

	public string Value { get; }


	// Use only in cases where you are sure that the userName is valid
	public static CommentContent CreateVerified(string commentContentStr) => new(commentContentStr);

	public static Result<CommentContent> Create(string commentContentStr)
	{
		if (string.IsNullOrWhiteSpace(commentContentStr))
		{
			return CommentErrors.EmptyContent();
		}

		if (commentContentStr.Length is < CommentConstants.ContentMinLength or > CommentConstants.ContentMaxLength)
		{
			return CommentErrors.InvalidContentLength(commentContentStr);
		}

		return new CommentContent(commentContentStr);
	}
}


