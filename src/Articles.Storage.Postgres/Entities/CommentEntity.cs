namespace Articles.Storage.Postgres.Entities;

public class CommentEntity
{
	public Guid Id { get; set; }

	public string Content { get; set; } = null!;

	public Guid AuthorId { get; set; }

	public UserEntity Author { get; set; } = null!;

	public Guid ArticleId { get; set; }

	public ArticleEntity Article { get; set; } = null!;
}
