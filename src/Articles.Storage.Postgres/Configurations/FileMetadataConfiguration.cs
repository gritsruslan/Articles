using Articles.Domain.Constants;
using Articles.Domain.ValueObjects;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

public sealed class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
	public void Configure(EntityTypeBuilder<FileMetadata> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.FileFormat)
			.HasConversion(x => x.ContentType, contentType => FileFormat.FromContentType(contentType).Value)
			.HasMaxLength(SupportedFileFormats.ContentTypeMaxLength);
	}
}
