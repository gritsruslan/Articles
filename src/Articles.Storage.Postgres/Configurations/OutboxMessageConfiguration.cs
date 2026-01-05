using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		builder.HasKey(o => o.Id);

		builder.Property(o => o.ContentBlob).HasColumnType("json");
	}
}
