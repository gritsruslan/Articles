using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
{
	public void Configure(EntityTypeBuilder<CommentEntity> builder)
	{
		builder.HasKey(e => e.Id);

		builder
			.Property(e => e.Content)
			.HasMaxLength(CommentConstants.ContentMaxLength);

		builder
			.HasOne(e => e.Article)
			.WithMany(a => a.Comments)
			.HasForeignKey(e => e.ArticleId);

		builder
			.HasOne(e => e.Author)
			.WithMany()
			.HasForeignKey(e => e.AuthorId);
	}
}
