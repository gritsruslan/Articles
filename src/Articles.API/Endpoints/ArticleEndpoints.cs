using Articles.API.Extensions;
using Articles.Application.ArticleUseCases.GetArticles;
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

		return app;
	}

	private static async Task<IResult> GetArticles(
		[FromQuery] string? searchQuery,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var query = new GetArticlesQuery(searchQuery, page, pageSize);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
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
