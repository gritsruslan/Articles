using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.CommentUseCases.DeleteComment;
using Articles.Application.CommentUseCases.UpdateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class CommentEndpoints
{
	public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("comments");

		group.MapPatch("{commentId:guid}", Update);
		group.MapDelete("{commentId:guid}", Delete);

		return app;
	}

	private static async Task<IResult> Update(
		[FromRoute] Guid commentId,
		[FromBody] UpdateCommentRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new UpdateCommentCommand(commentId, request.Content);
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
}
