using Articles.Domain.Constants;
using Articles.Domain.DomainEvents;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		builder.HasKey(o => o.Id);

		builder.Property(o => o.ContentBlob).HasColumnType("json");

		builder.Property(o => o.TraceId)
			.HasMaxLength(MonitoringConstants.TraceIdSize)
			.IsFixedLength();

		builder.Property(o => o.SpanId)
			.HasMaxLength(MonitoringConstants.SpanIdSize)
			.IsFixedLength();
	}
}
