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

	public Task<List<Domain.Models.FileMetadata>> GetUnlinked(int take, TimeSpan? olderThan, CancellationToken cancellationToken)
	{
		var query = dbContext.FileMetadata.Where(f => f.ArticleId == null);

		if (olderThan is not null)
		{
			query = query.Where(f => f.UploadedAt < DateTime.UtcNow - olderThan);
		}

		return query.OrderByDescending(f => f.UploadedAt)
			.Take(take)
			.Select(f => new Articles.Domain.Models.FileMetadata()
			{
				Id = f.Id,
				FileFormat = FileFormat.FromContentType(f.ContentType).Value,
				ArticleId = f.ArticleId,
				UploadedAt = f.UploadedAt
			})
			.ToListAsync(cancellationToken);
	}

	public Task LinkToArticle(IEnumerable<Guid> fileIds, ArticleId articleId, CancellationToken cancellationToken)
	{
		return dbContext.FileMetadata.Where(f => fileIds.Contains(f.Id))
			.ExecuteUpdateAsync(c => c.SetProperty(f => f.ArticleId, articleId.Value), cancellationToken);
	}

	public Task DeleteById(Guid fileId, CancellationToken cancellationToken)
	{
		return dbContext.FileMetadata.Where(f => f.Id == fileId).ExecuteDeleteAsync(cancellationToken);
	}
}
