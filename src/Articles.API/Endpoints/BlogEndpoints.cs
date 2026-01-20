using Articles.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class BlogEndpoints
{
	public static IEndpointRouteBuilder MapBlogEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("blogs");

		group.MapPost(string.Empty, CreateBlog);
		group.MapGet("{blogId:guid}", GetBlog);
		group.MapGet(string.Empty, GetBlogs);

		return group;
	}

	private static Task GetBlogs(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	// pagination
	// read model
	// sort by count of articles
	private static Task GetBlog([FromRoute] Guid blogId, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	private static Task CreateBlog([FromBody] CreateBlogRequest request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
