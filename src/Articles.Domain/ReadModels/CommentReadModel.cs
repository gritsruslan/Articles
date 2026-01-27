namespace Articles.Domain.ReadModels;

public sealed class CommentReadModel
{
	public Guid Id { get; set; }

	public string Content { get; set; } = null!;

	public Guid AuthorId { get; set; }

	public string AuthorName { get; set; } = null!;

	public DateTime CreatedAt { get; set; }
}
