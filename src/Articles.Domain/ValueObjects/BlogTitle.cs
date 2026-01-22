using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct BlogTitle
{
	private BlogTitle(string blogTitleStr) => Value = blogTitleStr;

	public string Value { get; }


	// Use only in cases where you are sure that the userName is valid
	public static BlogTitle CreateVerified(string blogTitleStr) => new(blogTitleStr);

	public static Result<BlogTitle> Create(string blogTitleStr)
	{
		if (string.IsNullOrWhiteSpace(blogTitleStr))
		{
			return BlogErrors.EmptyTitle(blogTitleStr);
		}

		if (blogTitleStr.Length is < BlogConstants.TitleMinLength or > BlogConstants.TitleMaxLength)
		{
			return BlogErrors.InvalidTitleLength(blogTitleStr);
		}

		return new BlogTitle(blogTitleStr);
	}
}

