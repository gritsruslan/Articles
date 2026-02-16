using Articles.Domain.Identifiers;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Models;

public sealed class Comment
{
	public CommentId Id { get; set; }

	public CommentContent Content { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public UserId AuthorId { get; set; }

	public ArticleId ArticleId { get; set; }
}
