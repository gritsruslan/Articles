using Articles.Shared.Abstraction;

namespace Articles.Storage.Postgres.Entities;

public sealed class ArticleEntity : BaseAuditableEntity
{
	public Guid Id { get; set; }

	public string Title { get; set; } = null!;

	public string Data { get; set; } = null!;

	public int BlogId { get; set; }

	public BlogEntity Blog { get; set; } = null!;

	public Guid AuthorId { get; set; }

	public UserEntity Author { get; set; } = null!;

	public ICollection<CommentEntity> Comments { get; set; } = null!;
}
