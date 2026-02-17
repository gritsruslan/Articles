using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.ArticleUseCases.DeleteArticle;
using Articles.Application.ArticleUseCases.GetArticleById;
using Articles.Application.ArticleUseCases.GetArticles;
using Articles.Application.CommentUseCases.CreateComment;
using Articles.Application.CommentUseCases.GetCommentsByArticle;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class ArticleEndpoints
{
	public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("articles")
			.RequireRateLimiting(SecurityExtensions.ApiRateLimitingPolicy);

		group.MapGet("{articleId:guid}", GetArticle);
		group.MapGet("search", GetArticles);
		group.MapDelete("{articleId:guid}", DeleteArticle);
		group.MapPost("{articleId:guid}/comments", CreateComment);
		group.MapGet("{articleId:guid}/comments", GetArticleComments);

		return app;
	}

	private static async Task<IResult> GetArticleComments(
		[FromRoute] Guid articleId,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var query = new GetCommentsByArticleQuery(articleId, page, pageSize);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> CreateComment(
		[FromRoute] Guid articleId,
		[FromBody] CreateCommentRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new CreateCommentCommand(articleId, request.Content);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		var comment = result.Value;
		return Results.Ok(comment);
	}

	private static async Task<IResult> GetArticles(
		[FromQuery] string searchQuery,
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

	// TODO ReadModel + Sort by likes count
	private static async Task<IResult> GetArticle(
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

		return Results.NoContent();
	}
}
