using Articles.Domain.Constants;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class BlogErrors
{
	public static Error BlogNotFound(BlogId id) =>
		new(ErrorType.NotFound,
			$"Blog with id {id.Value} was not found",
			"blog.not.found",
			"BlogId",
			id.Value.ToString());

	public static Error EmptyTitle() =>
		AbstractErrors.EmptyParameter("BlogTitle");

	public static Error InvalidTitleLength(string invalidTitle) =>
		AbstractErrors.InvalidParameterLength("BlogTitle", invalidTitle, BlogConstants.TitleMinLength, BlogConstants.TitleMaxLength);
}
