using Articles.Domain.Constants;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class ArticleErrors
{
	public static Error NotFound(ArticleId articleId) =>
		ErrorsFactory.ObjectNotFoundById("article", articleId.Value.ToString());

	public static Error NotAnAuthor() =>
		new(ErrorType.Forbidden,
			"You are not an author of this article",
			"not.an.author");

	public static Error EmptyTitle() =>
		ErrorsFactory.RequiredParameter("article-title");

	public static Error InvalidTitleLength(string invalidTitle) =>
		ErrorsFactory.InvalidParameterLength(
			"article-title",
			invalidTitle,
			ArticleConstants.TitleMinLength,
			ArticleConstants.TitleMaxLength);

	public static Error EmptyData() =>
		ErrorsFactory.RequiredParameter("article-data");

	public static Error InvalidDataLength() =>
		ErrorsFactory.InvalidParameterLength(
			"article-data",
			parameterValue: null, //ignore
			ArticleConstants.DataMinLength,
			ArticleConstants.DataMaxLength);
}
