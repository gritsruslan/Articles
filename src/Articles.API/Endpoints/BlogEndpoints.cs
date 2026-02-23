using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.ArticleUseCases.CreateArticle;
using Articles.Application.BlogUseCases.CreateBlog;
using Articles.Application.BlogUseCases.GetBlog;
using Articles.Application.BlogUseCases.GetBlogArticles;
using Articles.Application.BlogUseCases.GetBlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class BlogEndpoints
{
	public static IEndpointRouteBuilder MapBlogEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("blogs")
			.RequireRateLimiting(SecurityExtensions.ApiRateLimitingPolicy);

		group.MapGet("{blogId:int}", GetBlog);
		group.MapGet(string.Empty, GetBlogs);
		group.MapPost(string.Empty, CreateBlog);
		group.MapGet("{blogId:int}/articles", GetBlogArticles);
		group.MapPost("{blogId:int}/articles", CreateArticle);

		return app;
	}

	private static async Task<IResult> CreateArticle(
		[FromRoute] int blogId,
		[FromBody] CreateArticleRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new CreateArticleCommand(blogId, request.Title, request.Data, request.AttachedFiles);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		var articleId = result.Value;
		return Results.Created($"articles/{articleId}", null);
	}

	private static async Task<IResult> GetBlogArticles(
		[FromRoute] int blogId,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var query = new GetBlogArticlesQuery(blogId, page, pageSize);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> GetBlogs(
		[FromServices] ISender sender,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		CancellationToken cancellationToken)
	{
		var query = new GetBlogsQuery(page, pageSize);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> GetBlog(
		[FromServices] ISender sender,
		[FromRoute] int blogId,
		CancellationToken cancellationToken)
	{
		var query = new GetBlogQuery(blogId);
		var result = await sender.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> CreateBlog(
		[FromServices] ISender sender,
		[FromBody] CreateBlogRequest request,
		CancellationToken cancellationToken)
	{
		var command = new CreateBlogCommand(request.Title);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		var blogId = result.Value;
		return Results.Created($"blogs/{blogId}", null);
	}
}
