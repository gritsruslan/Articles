using Articles.Domain.Identifiers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class ArticleEndpoints
{
	public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("articles");

		group.MapPost(string.Empty, CreateArticle);
		group.MapGet("{articleId:guid}", GetArticleById);
		group.MapGet("{blogId:guid}", GetArticlesByBlog);
		group.MapGet(string.Empty, GetArticles);

		return group;
	}

	private static Task GetArticles(
		[FromQuery] string? query,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	private static Task GetArticlesByBlog()
	{
		throw new NotImplementedException();
	}

	private static Task GetArticleById()
	{
		throw new NotImplementedException();
	}

	private static Task CreateArticle()
	{
		throw new NotImplementedException();
	}
}
