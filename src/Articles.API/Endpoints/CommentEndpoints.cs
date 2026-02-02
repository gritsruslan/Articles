using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.CommentUseCases.CreateComment;
using Articles.Application.CommentUseCases.DeleteComment;
using Articles.Application.CommentUseCases.GetCommentsByArticle;
using Articles.Application.CommentUseCases.UpdateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class CommentEndpoints
{
	public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("comments");

		group.MapGet(string.Empty, Get);
		group.MapPost(string.Empty, Create);
		group.MapPatch(string.Empty, Update);
		group.MapDelete("{commentId:guid}", Delete);

		return app;
	}

	private static async Task<IResult> Update(
		[FromBody] UpdateCommentRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new UpdateCommentCommand(request.CommentId, request.Content);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok();
	}

	private static async Task<IResult> Delete(
		[FromRoute] Guid commentId,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new DeleteCommentCommand(commentId);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.NoContent();
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

		var comment = result.Value;
		return Results.Ok(comment);
	}

	private static async Task<IResult> Get(
		[FromQuery] Guid articleId,
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
}
