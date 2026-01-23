using Articles.Shared.Abstraction;

namespace Articles.Storage.Postgres.Entities;

public sealed class ArticleEntity : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public string Title { get; set; } = null!;

	public string Data { get; set; } = null!;

	public int BlogId { get; set; }

	public BlogEntity Blog { get; set; } = null!;

	public Guid CreatorId { get; set; }

	public UserEntity Creator { get; set; } = null!;
}
