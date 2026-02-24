using Articles.Domain.Constants;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class BlogErrors
{
	public static Error NotFound(BlogId blogId) =>
		ErrorsFactory.ObjectNotFoundById("blog", blogId.Value.ToString());

	public static Error EmptyTitle() =>
		ErrorsFactory.RequiredParameter("blog-title");

	public static Error InvalidTitleLength(string invalidTitle) =>
		ErrorsFactory.InvalidParameterLength(
			"blog-title",
			invalidTitle,
			BlogConstants.TitleMinLength,
			BlogConstants.TitleMaxLength);
}
