using Articles.Domain.DomainEvents;
using Articles.Storage.Postgres.Entities;
using FileMetadata = Articles.Storage.Postgres.Entities.FileMetadata;

namespace Articles.Storage.Postgres;

public sealed class ArticlesDbContext(DbContextOptions<ArticlesDbContext> options) : DbContext(options)
{
	public DbSet<UserEntity> Users => Set<UserEntity>();

	public DbSet<SessionEntity> Sessions => Set<SessionEntity>();

	public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

	public DbSet<FileMetadata> FileMetadata => Set<FileMetadata>();

	public DbSet<BlogEntity> Blogs => Set<BlogEntity>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//modelBuilder.HasDefaultSchema("articles");
		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyMarker.Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
