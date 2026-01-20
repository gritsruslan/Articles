using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Articles.Infrastructure.BackgroundService;

// very simple file cleaner background service
// maybe rewrite with domain events
public sealed class FileCleanerBackgroundService(
	ILogger<FileCleanerBackgroundService> logger,
	IServiceProvider serviceProvider) : Microsoft.Extensions.Hosting.BackgroundService
{
	private readonly TimeSpan _delay = TimeSpan.FromHours(6);

	private const int BatchSize = 10;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await Task.Yield();
		logger.LogInformation("== Starting FileCleanerBackgroundService ==");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var serviceScope = serviceProvider.CreateScope();
				var metadataRepository = serviceScope.ServiceProvider.GetRequiredService<IFileMetadataRepository>();
				var fileRepository = serviceScope.ServiceProvider.GetRequiredService<IFileRepository>();

				var unlinkedFileMetadata = await metadataRepository.GetUnlinked(
					BatchSize, TimeSpan.FromHours(6), stoppingToken);

				int cleaned = 0;
				foreach (var fileMetadata in unlinkedFileMetadata)
				{
					var bucketResult = FileBucketNames.FromFormat(fileMetadata.FileFormat);
					if (bucketResult.IsFailure)
					{
						logger.LogError("Unable to resolve bucket for file {fileId} with content type {contentType}",
							fileMetadata.Id, fileMetadata.FileFormat.ContentType );
						continue;
					}

					await fileRepository.DeleteFile(bucketResult.Value, fileMetadata.Id.ToString(), stoppingToken);
					await metadataRepository.DeleteById(fileMetadata.Id, stoppingToken);

					cleaned++;
				}

				logger.LogInformation("Successfully cleaned {cleaned} unlinked files in FileCleanerBackgroundService", cleaned);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unhandled exception in FileCleanerBackgroundService");
				throw;
			}

			await Task.Delay(_delay, stoppingToken);
		}
	}
}
