using Articles.API.Extensions;
using Articles.API.Handlers;
using Articles.API.Requests;
using Articles.Application.UseCases.Blogs.CreateArticle;
using Articles.Application.UseCases.Blogs.CreateBlog;
using Articles.Application.UseCases.Blogs.GetBlog;
using Articles.Application.UseCases.Blogs.GetBlogArticles;
using Articles.Application.UseCases.Blogs.GetBlogs;
using Articles.Domain.Models;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;
using Articles.Shared.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace Articles.API.Endpoints;

internal static class BlogEndpoints
{
	public static IEndpointRouteBuilder MapBlogEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("blogs")
			.RequireRateLimiting(SecurityExtensions.ApiRateLimitingPolicy);

		group.MapGet("{blogId:int}", GetBlog);
		group.MapGet(string.Empty, GetBlogReadModels);
		group.MapPost(string.Empty, CreateBlog);
		group.MapGet("{blogId:int}/articles", GetBlogArticles);
		group.MapPost("{blogId:int}/articles", CreateArticle);

		return app;
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<Error>(StatusCodes.Status422UnprocessableEntity)]
	private static async Task<IResult> CreateArticle(
		[FromRoute] int blogId,
		[FromBody] CreateArticleRequest request,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new CreateArticleCommand(blogId, request.Title, request.Data, request.AttachedFiles);

		return await handler.Handle(
			command,
			id => Results.Created($"articles/{id}", null),
			cancellationToken);
	}

	[ProducesResponseType<PagedData<ArticleSearchReadModel>>(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<Error>(StatusCodes.Status422UnprocessableEntity)]
	private static async Task<IResult> GetBlogArticles(
		[FromRoute] int blogId,
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetBlogArticlesQuery(blogId, page, pageSize);
		return await handler.Handle(query, Results.Ok, cancellationToken);
	}

	[ProducesResponseType<PagedData<BlogReadModel>>(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status422UnprocessableEntity)]
	private static async Task<IResult> GetBlogReadModels(
		[FromQuery] int page,
		[FromQuery] int pageSize,
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetBlogsQuery(page, pageSize);
		return await handler.Handle(query, Results.Ok, cancellationToken);
	}

	[ProducesResponseType<Blog>(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
	private static async Task<IResult> GetBlog(
		[FromRoute] int blogId,
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetBlogQuery(blogId);
		return await handler.Handle(query, Results.Ok, cancellationToken);
	}

	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType<Error>(StatusCodes.Status403Forbidden)]
	[ProducesResponseType<Error>(StatusCodes.Status422UnprocessableEntity)]
	private static async Task<IResult> CreateBlog(
		[FromBody] CreateBlogRequest request,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new CreateBlogCommand(request.Title);
		return await handler.Handle(
			command,
			blogId => Results.Created($"blogs/{blogId}", null),
			cancellationToken);
	}
}
