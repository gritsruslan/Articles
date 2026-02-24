using Articles.API.Handlers;
using Articles.Application.FileUseCases.GetFile;
using Articles.Application.FileUseCases.UploadFile;
using Articles.Shared.Result;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class FileEndpoints
{
	public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("files").DisableAntiforgery();

		group.MapGet(string.Empty, GetFile);
		group.MapPost(string.Empty, UploadFile);

		return app;
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
	private static async Task<IResult> GetFile(
		string fileName,
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		var query = new GetFileQuery(fileName);
		return await handler.Handle(query,
			response => Results.File(response.FileStream, response.ContentType),
			cancellationToken);
	}


	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType<Error>(StatusCodes.Status413PayloadTooLarge)]
	[ProducesResponseType<Error>(StatusCodes.Status415UnsupportedMediaType)]
	private static async Task<IResult> UploadFile(
		IFormFile file,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new UploadFileCommand(file);
		return await handler.Handle(
			command,
			fileName => Results.Created($"files/{fileName}", null),
			cancellationToken);
	}
}
