namespace Articles.Application.Interfaces.Repositories;

public interface IFileMetadataRepository
{
	Task<bool> Exists(Guid fileId, CancellationToken cancellationToken);

	Task Add(FileMetadata fileMetadata, CancellationToken cancellationToken);

	Task SoftDeleteByArticleId(Guid articleId, CancellationToken cancellationToken);
}
