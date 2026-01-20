namespace Articles.Application.Interfaces.Repositories;

public interface IFileMetadataRepository
{
	Task<bool> Exists(Guid fileId, CancellationToken cancellationToken);

	Task Add(FileMetadata fileMetadata, CancellationToken cancellationToken);

	Task<List<FileMetadata>> GetUnlinked(int take, TimeSpan? olderThan, CancellationToken cancellationToken);

	Task DeleteById(Guid fileId, CancellationToken cancellationToken);
}
