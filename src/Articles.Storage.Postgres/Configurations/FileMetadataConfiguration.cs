using Articles.Domain.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FileMetadata = Articles.Storage.Postgres.Entities.FileMetadata;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class FileMetadataConfiguration : IEntityTypeConfiguration<FileMetadata>
{
	public void Configure(EntityTypeBuilder<FileMetadata> builder)
	{
		builder.HasKey(fm => fm.Id);

		builder.Property(fm => fm.ContentType)
			.HasMaxLength(FileFormats.ContentTypeMaxLength);
	}
}
