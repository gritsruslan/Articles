using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct ArticleTitle
{
	private ArticleTitle(string articleTitle) => Value = articleTitle;

	public string Value { get; }


	// Use only in cases where you are sure that the article title is valid
	public static ArticleTitle CreateVerified(string articleTitle) => new(articleTitle);

	public static Result<ArticleTitle> Create(string articleTitle)
	{
		if (string.IsNullOrWhiteSpace(articleTitle))
		{
			return ArticleErrors.EmptyTitle();
		}

		if (articleTitle.Length is < ArticleConstants.TitleMinLength or > ArticleConstants.TitleMaxLength)
		{
			return ArticleErrors.InvalidTitleLength(articleTitle);
		}

		return new ArticleTitle(articleTitle);
	}
}
