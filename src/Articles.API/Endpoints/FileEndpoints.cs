using Articles.API.Extensions;
using Articles.Application.FileUseCases.GetFile;
using Articles.Application.FileUseCases.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class FileEndpoints
{
	public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("file").DisableAntiforgery();

		group.MapGet(string.Empty, GetFile);
		group.MapPost(string.Empty, UploadFile);
		group.MapDelete(string.Empty, DeleteFile);

		return app;
	}

	private static async Task<IResult> GetFile(
		[FromServices] ISender sender,
		string fileName,
		CancellationToken cancellationToken)
	{
		var command = new GetFileQuery(fileName);

		var result = await sender.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.File(result.Value.FileStream, result.Value.ContentType);
	}

	private static async Task<IResult> UploadFile(
		[FromServices] ISender sender,
		IFormFile file,
		CancellationToken cancellationToken)
	{
		var command = new UploadFileCommand(file);
		var result = await sender.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok();
	}

	private static Task<IResult> DeleteFile(
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
