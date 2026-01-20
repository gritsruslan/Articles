using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class BlogEntityConfiguration : IEntityTypeConfiguration<BlogEntity>
{
	public void Configure(EntityTypeBuilder<BlogEntity> builder)
	{
		builder.HasKey(b => b.Id);

		builder.Property(b => b.Title)
			.HasMaxLength(BlogConstants.TitleMaxLength);
	}
}
