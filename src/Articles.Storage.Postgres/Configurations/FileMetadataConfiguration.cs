using Articles.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FileMetadata = Articles.Storage.Postgres.Entities.FileMetadata;

namespace Articles.Storage.Postgres.Configurations;

public sealed class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
	public void Configure(EntityTypeBuilder<FileMetadata> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.ContentType)
			.HasMaxLength(SupportedFileFormats.ContentTypeMaxLength);
	}
}
