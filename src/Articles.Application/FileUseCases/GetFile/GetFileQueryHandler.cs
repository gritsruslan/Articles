using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;

namespace Articles.Application.FileUseCases.GetFile;

internal sealed class GetFileQueryHandler(IFileRepository fileRepository) : IQueryHandler<GetFileQuery, GetFileResponse>
{
	public async Task<Result<GetFileResponse>> Handle(GetFileQuery request, CancellationToken cancellationToken)
	{
		//TODO : DB ?
		var fullFileName = request.FileName;
		var extension = Path.GetExtension(fullFileName);

		var contentTypeQualify = SupportedFileFormats.FromExtension(extension);
		if (contentTypeQualify.IsFailure)
		{
			return contentTypeQualify.Error;
		}
		var contentType = contentTypeQualify.Value;

		var qualifyBucket = FileBucketNames.FromContentType(contentType);

		if (qualifyBucket.IsFailure)
		{
			return qualifyBucket.Error;
		}

		var fileName = Path.GetFileNameWithoutExtension(fullFileName);

		var stream = await fileRepository.GetFile(qualifyBucket.Value, fileName, cancellationToken);
		return new GetFileResponse(stream, contentTypeQualify.Value);
	}
}
