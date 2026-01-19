using Articles.Domain.ValueObjects;

namespace Articles.Storage.Postgres.Entities;

public sealed class FileMetadata
{
	public Guid Id { get; set; } // also name

	public FileFormat FileFormat { get; set; } = null!;

	public Guid? ArticleId { get; set; }

	public DateTime UploadedAt { get; set; }
}
