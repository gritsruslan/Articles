using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
	public void Configure(EntityTypeBuilder<SessionEntity> builder)
	{
		builder.ToTable("Sessions");

		builder.HasKey(s => s.Id);

		builder.HasOne(s => s.User)
			.WithMany(u => u.Sessions)
			.HasForeignKey(s => s.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(s => s.UserAgent)
			.HasMaxLength(SecurityConstants.UserAgentMaxLength);
	}
}
