using Articles.Domain.Constants;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class ArticleErrors
{
	public static Error ArticleNotFound(ArticleId id) =>
		new(ErrorType.NotFound,
			$"Article with id {id.Value} was not found",
			"article.not.found",
			"ArticleId",
			id.Value.ToString());

	public static Error EmptyTitle() =>
		AbstractErrors.EmptyParameter("ArticleTitle");

	public static Error InvalidTitleLength(string invalidTitle) =>
		AbstractErrors.InvalidParameterLength(
			"ArticleTitle",
			invalidTitle,
			ArticleConstants.TitleMinLength,
			ArticleConstants.TitleMaxLength);

	public static Error EmptyData() =>
		AbstractErrors.EmptyParameter("ArticleData");

	public static Error InvalidDataLength() =>
		AbstractErrors.InvalidParameterLength(
			"ArticleData",
			parameterValue: null,
			ArticleConstants.DataMinLength,
			ArticleConstants.DataMaxLength);
}
