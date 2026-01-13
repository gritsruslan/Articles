using Articles.Domain.Constants;
using Articles.Domain.DomainEvents;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		builder.HasKey(o => o.Id);

		builder.Property(o => o.ContentBlob).HasColumnType("json");

		builder.Property(o => o.TraceId)
			.HasMaxLength(OverallConstants.TraceIdSize)
			.IsFixedLength();

		builder.Property(o => o.SpanId)
			.HasMaxLength(OverallConstants.SpanIdSize)
			.IsFixedLength();
	}
}
