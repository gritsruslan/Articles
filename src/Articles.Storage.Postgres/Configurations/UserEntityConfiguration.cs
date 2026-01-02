using Articles.Domain.Constants;
using Articles.Storage.Postgres.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Articles.Storage.Postgres.Configurations;

internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
	public void Configure(EntityTypeBuilder<UserEntity> builder)
	{
		builder.ToTable("Users");

		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id).ValueGeneratedNever();

		builder.HasIndex(u => u.Email).IsUnique();
		builder.Property(u => u.Email).HasMaxLength(UserConstants.EmailMaxLength);

		builder.Property(u => u.Name).HasMaxLength(UserConstants.NameMaxLength);

		builder.HasIndex(u => u.DomainId).IsUnique();
		builder.Property(u => u.DomainId).HasMaxLength(UserConstants.DomainIdMaxLength);

		builder.Property(u => u.EmailConfirmed).HasDefaultValue(false);

		builder.Property(u => u.PasswordHash)
			.HasMaxLength(SecurityConstants.PasswordHashLength)
			.IsFixedLength();

		builder.Property(u => u.Salt)
			.HasMaxLength(SecurityConstants.SaltLength)
			.IsFixedLength();
	}
}
