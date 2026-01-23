using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class ArticleEntityConfiguration : IEntityTypeConfiguration<ArticleEntity>
{
	public void Configure(EntityTypeBuilder<ArticleEntity> builder)
	{
		builder.HasKey(x => x.Id);

		builder
			.Property(x => x.Title)
			.HasMaxLength(ArticleConstants.TitleMaxLength);

		builder
			.Property(x => x.Data)
			.HasMaxLength(ArticleConstants.DataMaxLength);

		builder
			.HasOne(x => x.Creator)
			.WithMany()
			.HasForeignKey(x => x.CreatorId);

		builder
			.HasOne(x => x.Blog)
			.WithMany()
			.HasForeignKey(x => x.BlogId);
	}
}
