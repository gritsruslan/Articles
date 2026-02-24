using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Files.GetFile;

internal sealed class GetFileQueryHandler(
	IFileRepository fileRepository,
	IFileMetadataRepository metadataRepository) : IQueryHandler<GetFileQuery, GetFileResponse>
{
	public async Task<Result<GetFileResponse>> Handle(GetFileQuery request, CancellationToken cancellationToken)
	{
		var fullFileName = request.FileName;
		var formatResult = FileFormat.FromExtension(Path.GetExtension(fullFileName));
		if (formatResult.IsFailure)
		{
			return formatResult.Error;
		}
		var format = formatResult.Value;

		var qualifyBucket = FileBucketNames.FromFormat(format);
		if (qualifyBucket.IsFailure)
		{
			return qualifyBucket.Error;
		}

		var fileName = Path.GetFileNameWithoutExtension(fullFileName);
		var fileId = Guid.Parse(fileName);

		var exists = await metadataRepository.Exists(fileId, cancellationToken);
		if (!exists)
		{
			return FileErrors.FileNotFound(fullFileName);
		}

		var stream = await fileRepository.GetFile(qualifyBucket.Value, fileName, cancellationToken);
		return new GetFileResponse(stream, format.ContentType);
	}
}
