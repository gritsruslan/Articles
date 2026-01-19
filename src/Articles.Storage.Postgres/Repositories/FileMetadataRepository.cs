using FileMetadata = Articles.Storage.Postgres.Entities.FileMetadata;

namespace Articles.Storage.Postgres.Repositories;

public sealed class FileMetadataRepository(ArticlesDbContext dbContext) : IFileMetadataRepository
{
	public Task<bool> Exists(Guid fileId, CancellationToken cancellationToken)
	{
		return dbContext.FileMetadata.AnyAsync(f => f.Id == fileId, cancellationToken);
	}

	public async Task Add(Articles.Domain.Models.FileMetadata fileMetadata, CancellationToken cancellationToken)
	{
		dbContext.FileMetadata.Add(new FileMetadata
		{
			Id = fileMetadata.Id,
			ContentType = fileMetadata.FileFormat.ContentType,
			ArticleId = fileMetadata.ArticleId,
			UploadedAt = fileMetadata.UploadedAt
		});

		await dbContext.SaveChangesAsync(cancellationToken);
	}

	public Task SoftDeleteByArticleId(Guid articleId, CancellationToken cancellationToken)
	{
		return dbContext.FileMetadata
			.Where(f => f.ArticleId == articleId)
			.ExecuteUpdateAsync(x => x.SetProperty(f => f.ArticleId, (Guid?)null), cancellationToken);
	}
}
