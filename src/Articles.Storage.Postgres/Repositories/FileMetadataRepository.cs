using Articles.Domain.ValueObjects;
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

	public Task<List<Articles.Domain.Models.FileMetadata>> GetUnlinked(CancellationToken cancellationToken)
	{
		return dbContext.FileMetadata
			.Where(f => f.ArticleId == null)
			.Select(f => new Articles.Domain.Models.FileMetadata()
			{
				Id = f.Id,
				FileFormat = FileFormat.FromContentType(f.ContentType).Value,
				ArticleId = f.ArticleId,
				UploadedAt = f.UploadedAt
			})
			.ToListAsync(cancellationToken);
	}

	public Task BatchDeleteByIds(List<Guid> fileMetadataIds, CancellationToken cancellationToken)
	{
		return dbContext.FileMetadata
			 .Where(f => fileMetadataIds.Contains(f.Id))
			 .ExecuteDeleteAsync(cancellationToken: cancellationToken);
	}
}
