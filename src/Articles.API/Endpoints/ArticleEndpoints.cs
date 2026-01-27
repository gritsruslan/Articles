using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.ArticleUseCases.CreateArticle;
using Articles.Application.ArticleUseCases.DeleteArticle;
using Articles.Application.ArticleUseCases.GetArticleById;
using Articles.Application.ArticleUseCases.GetArticles;
using Articles.Application.ArticleUseCases.GetArticlesByBlog;
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
		group.MapGet("by-blog/{blogId:int}", GetArticlesByBlog);
		group.MapGet(string.Empty, GetArticles);
		group.MapDelete("{articleId:guid}", DeleteArticle);

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

	private static async Task<IResult> CreateArticle(
		[FromBody] CreateArticleRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new CreateArticleCommand(request.BlogId, request.Title, request.Data, request.AttachedFiles);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> GetArticlesByBlog(
		[FromRoute] int blogId,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var query = new GetArticlesByBlogQuery(blogId, page, pageSize);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	// TODO ReadModel + Sort by likes count
	private static async Task<IResult> GetArticleById(
		[FromRoute] Guid articleId,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var query = new GetArticleByIdQuery(articleId);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> DeleteArticle(
		[FromRoute] Guid articleId,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new DeleteArticleCommand(articleId);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok();
	}

}
