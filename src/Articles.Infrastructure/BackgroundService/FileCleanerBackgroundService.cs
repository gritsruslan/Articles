using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;
using Articles.Shared.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Articles.Infrastructure.BackgroundService;

public sealed class FileCleanerBackgroundService(
	ILogger<FileCleanerBackgroundService> logger,
	IFileMetadataRepository metadataRepository,
	IFileRepository fileRepository,
	IUnitOfWork unitOfWork) : Microsoft.Extensions.Hosting.BackgroundService
{
	private readonly TimeSpan _delay = TimeSpan.FromHours(6);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		// TODO : refactoring
		await Task.Yield();

		await using var scope = await unitOfWork.StartScope(stoppingToken);

		var unlinkedMetadatas = await metadataRepository.GetUnlinked(stoppingToken);
		// delete files that are unlinked and older than 6 hours
		var forDelete = unlinkedMetadatas.Where(f =>
			f.UploadedAt < DateTime.UtcNow.Add(TimeSpan.FromHours(-6)));
		var forDeleteIds = forDelete.Select(f => f.Id).ToList();

		// for every bucket
		var imagesForDelete = forDelete.Where(f =>
		{
			var bucket = FileBucketNames.FromFormat(f.FileFormat);
			if (bucket.IsFailure)
			{
				return false;
			}
			return bucket.Value == FileBucketNames.Images;
		}).Select(f => f.Id.ToString()).ToList();

		var videosForDelete = forDelete.Where(f =>
		{
			var bucket = FileBucketNames.FromFormat(f.FileFormat);
			if (bucket.IsFailure)
			{
				return false;
			}
			return bucket.Value == FileBucketNames.Videos;
		}).Select(f => f.Id.ToString()).ToList();

		var otherForDelete = forDelete.Where(f =>
		{
			var bucket = FileBucketNames.FromFormat(f.FileFormat);
			if (bucket.IsFailure)
			{
				return false;
			}
			return bucket.Value == FileBucketNames.Other;
		}).Select(f => f.Id.ToString()).ToList();

		await metadataRepository.BatchDeleteByIds(forDeleteIds, stoppingToken);

		await fileRepository.DeleteFiles(FileBucketNames.Images,imagesForDelete, stoppingToken);
		await fileRepository.DeleteFiles(FileBucketNames.Videos, videosForDelete, stoppingToken);
		await fileRepository.DeleteFiles(FileBucketNames.Other, otherForDelete, stoppingToken);

		await scope.Commit(stoppingToken);

		logger.LogInformation("File Cleaner Service : cleaned");

		await Task.Delay(_delay, stoppingToken);
	}
}
