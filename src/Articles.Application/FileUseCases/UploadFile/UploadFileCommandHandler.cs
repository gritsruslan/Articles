using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;
using Articles.Shared.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Articles.Application.FileUseCases.UploadFile;

internal sealed class UploadFileCommandHandler(
	IFileRepository fileRepository,
	ILogger<UploadFileCommandHandler> logger,
	IFileMetadataRepository metadataRepository,
	IUnitOfWork unitOfWork) :
	ICommandHandler<UploadFileCommand, string>
{
	public async Task<Result<string>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
	{
		var file = request.File;
		var fileNewName = Guid.NewGuid().ToString();

		var formatResult = FileFormat.FromContentType(file.ContentType);
		if (formatResult.IsFailure)
		{
			return formatResult.Error;
		}
		var format = formatResult.Value;

		var fileFullName = fileNewName + format.Extension;

		var bucketResult = FileBucketNames.FromFormat(format);
		if (bucketResult.IsFailure)
		{
			return bucketResult.Error;
		}

		if (file.Length > SupportedFileFormats.MaxFileSize)
		{
			return FileErrors.TooLargeFile();
		}

		await using var scope = await unitOfWork.StartScope(cancellationToken);

		await metadataRepository.Add(new FileMetadata
		{
			Id = Guid.Parse(fileNewName),
			FileFormat = format,
			UploadedAt = DateTime.UtcNow,
			ArticleId = null
		}, cancellationToken);
		await fileRepository.UploadFile(
			bucketResult.Value, file.OpenReadStream(), fileNewName, file.ContentType);

		await scope.Commit(cancellationToken);

		logger.LogInformation("File Uploaded : {fileNewName}", fileNewName);

		return fileFullName;
	}
}
