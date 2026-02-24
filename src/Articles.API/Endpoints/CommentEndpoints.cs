using Articles.API.Extensions;
using Articles.API.Handlers;
using Articles.API.Requests;
using Articles.Application.UseCases.Comments.DeleteComment;
using Articles.Application.UseCases.Comments.UpdateComment;
using Articles.Shared.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class CommentEndpoints
{
	public static IEndpointRouteBuilder MapCommentEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("comments")
			.RequireRateLimiting(SecurityExtensions.ApiRateLimitingPolicy);

		group.MapPatch("{commentId:guid}", Update);
		group.MapDelete("{commentId:guid}", Delete);

		return app;
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status403Forbidden)]
	[ProducesResponseType<Error>(StatusCodes.Status422UnprocessableEntity)]
	private static async Task<IResult> Update(
		[FromRoute] Guid commentId,
		[FromBody] UpdateCommentRequest request,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new UpdateCommentCommand(commentId, request.Content);
		return await handler.Handle(command, Results.Ok, cancellationToken);
	}

	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<Error>(StatusCodes.Status403Forbidden)]
	[ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
	private static async Task<IResult> Delete(
		[FromRoute] Guid commentId,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new DeleteCommentCommand(commentId);
		return await handler.Handle(command, Results.NoContent, cancellationToken);
	}
}
