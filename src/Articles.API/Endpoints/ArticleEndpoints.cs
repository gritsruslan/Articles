using Articles.API.Extensions;
using Articles.API.Handlers;
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
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetCommentsByArticleQuery(articleId, page, pageSize);
		return await handler.Handle(query, Results.Ok, cancellationToken);
	}

	private static async Task<IResult> CreateComment(
		[FromRoute] Guid articleId,
		[FromBody] CreateCommentRequest request,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new CreateCommentCommand(articleId, request.Content);
		return await handler.Handle(command, Results.Ok, cancellationToken);
	}

	private static async Task<IResult> GetArticles(
		[FromQuery] string searchQuery,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetArticlesQuery(searchQuery, page, pageSize);
		return await handler.Handle(query, Results.Ok, cancellationToken);
	}

	private static async Task<IResult> GetArticle(
		[FromRoute] Guid articleId,
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetArticleByIdQuery(articleId);
		return await handler.Handle(query, Results.Ok, cancellationToken);
	}

	private static async Task<IResult> DeleteArticle(
		[FromRoute] Guid articleId,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new DeleteArticleCommand(articleId);
		return await handler.Handle(command, Results.Ok, cancellationToken);
	}
}
