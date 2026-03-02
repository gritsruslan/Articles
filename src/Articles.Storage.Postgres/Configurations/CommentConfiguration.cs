using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
{
	public void Configure(EntityTypeBuilder<CommentEntity> builder)
	{
		builder.HasKey(c => c.Id);

		builder
			.Property(c => c.Content)
			.HasMaxLength(CommentConstants.ContentMaxLength);

		builder
			.HasOne(c => c.Article)
			.WithMany(a => a.Comments)
			.HasForeignKey(c => c.ArticleId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(c => c.Author)
			.WithMany()
			.HasForeignKey(c => c.AuthorId);
	}
}
