using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;
using Microsoft.Extensions.Logging;

namespace Articles.Application.FileUseCases.UploadFile;

internal sealed class UploadFileCommandHandler(IFileRepository fileRepository,
	ILogger<UploadFileCommandHandler> logger) :
	ICommandHandler<UploadFileCommand>
{
	public async Task<Result> Handle(UploadFileCommand request, CancellationToken cancellationToken)
	{
		//TODO : DB, max size validation
		var file = request.File;
		var fileNewName = Guid.NewGuid().ToString();

		var formatResult = FileFormat.FromContentType(file.ContentType);
		if (formatResult.IsFailure)
		{
			return formatResult.Error;
		}
		var format = formatResult.Value;

		var bucketResult = FileBucketNames.FromFormat(format);
		if (bucketResult.IsFailure)
		{
			return bucketResult.Error;
		}

		if (file.Length > SupportedFileFormats.MaxFileSize)
		{
			return FileErrors.TooLargeFile();
		}

		await fileRepository.UploadFile(
			bucketResult.Value, file.OpenReadStream(), fileNewName, file.ContentType);

		logger.LogInformation("File Uploaded : {fileNewName}", fileNewName);

		return Result.Success();
	}
}
