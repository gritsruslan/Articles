using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.BlogUseCases.CreateBlog;
using Articles.Application.BlogUseCases.GetBlog;
using Articles.Application.BlogUseCases.GetBlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class BlogEndpoints
{
	public static IEndpointRouteBuilder MapBlogEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("blogs");

		group.MapPost(string.Empty, CreateBlog);
		group.MapGet("{blogId:int}", GetBlog);
		group.MapGet(string.Empty, GetBlogs);

		return app;
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

		return Results.Ok();
	}
}
