namespace Articles.Storage.Postgres.Entities;

public sealed class FileMetadata
{
	public Guid Id { get; set; } // also name

	public string ContentType { get; set; } = null!;

	public Guid? ArticleId { get; set; }

	public DateTime UploadedAt { get; set; }
}
