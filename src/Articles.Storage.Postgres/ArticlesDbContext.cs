using Articles.Storage.Postgres.Entities;

namespace Articles.Storage.Postgres;

public sealed class ArticlesDbContext(DbContextOptions<ArticlesDbContext> options) : DbContext(options)
{
	public DbSet<UserEntity> Users => Set<UserEntity>();

	public DbSet<SessionEntity> Sessions => Set<SessionEntity>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//modelBuilder.HasDefaultSchema("articles");
		modelBuilder.ApplyConfigurationsFromAssembly(AssemblyMarker.Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
