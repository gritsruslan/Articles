using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class ArticleEntityConfiguration : IEntityTypeConfiguration<ArticleEntity>
{
	public void Configure(EntityTypeBuilder<ArticleEntity> builder)
	{
		builder.HasKey(a => a.Id);

		builder
			.Property(a => a.Title)
			.HasMaxLength(ArticleConstants.TitleMaxLength);

		builder
			.Property(a => a.Data)
			.HasMaxLength(ArticleConstants.DataMaxLength);

		builder
			.HasOne(a => a.Author)
			.WithMany()
			.HasForeignKey(a => a.AuthorId);

		builder
			.HasOne(a => a.Blog)
			.WithMany()
			.HasForeignKey(a => a.BlogId);
	}
}
