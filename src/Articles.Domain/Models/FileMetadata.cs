using Articles.Domain.ValueObjects;

namespace Articles.Domain.Models;

public sealed class FileMetadata
{
	public Guid Id { get; set; }

	public FileFormat FileFormat { get; set; } = null!;

	public Guid? ArticleId { get; set; }

	public DateTime UploadedAt { get; set; }

	public string FullName => Id + FileFormat.Extension;
}
