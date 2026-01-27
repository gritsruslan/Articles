using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.CommentUseCases.CreateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class CommentEndpoints
{
	public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("comments");

		group.MapGet("{articleId:guid}", Get);
		group.MapPost(string.Empty, Create);
		group.MapDelete(string.Empty, Delete);

		return app;
	}

	private static Task Delete(HttpContext context)
	{
		throw new NotImplementedException();
	}

	private static async Task<IResult> Create(
		[FromBody] CreateCommentRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new CreateCommentCommand(request.ArticleId, request.Content);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok();
	}

	private static Task Get(HttpContext context)
	{
		throw new NotImplementedException();
	}
}
