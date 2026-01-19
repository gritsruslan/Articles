using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;

namespace Articles.Application.FileUseCases.GetFile;

internal sealed class GetFileQueryHandler(IFileRepository fileRepository) : IQueryHandler<GetFileQuery, GetFileResponse>
{
	public async Task<Result<GetFileResponse>> Handle(GetFileQuery request, CancellationToken cancellationToken)
	{
		//TODO : DB ?
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

		var stream = await fileRepository.GetFile(qualifyBucket.Value, fileName, cancellationToken);
		return new GetFileResponse(stream, format.ContentType);
	}
}
