using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct ArticleData
{
	private ArticleData(string articleData) => Value = articleData;

	public string Value { get; }


	// Use only in cases where you are sure that the article title is valid
	public static ArticleData CreateVerified(string articleData) => new(articleData);

	public static Result<ArticleData> Create(string articleData)
	{
		if (string.IsNullOrWhiteSpace(articleData))
		{
			return ArticleErrors.EmptyData();
		}

		if (articleData.Length is < ArticleConstants.DataMinLength or > ArticleConstants.DataMaxLength)
		{
			return ArticleErrors.InvalidDataLength();
		}

		return new ArticleData(articleData);
	}
}
