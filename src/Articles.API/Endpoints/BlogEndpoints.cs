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

		return group;
	}

	// pagination
	// read model
	// sort by count of articles
	private static async Task<IResult> GetBlogs([FromServices] ISender sender, CancellationToken cancellationToken)
	{
		var query = new GetBlogsQuery();
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
