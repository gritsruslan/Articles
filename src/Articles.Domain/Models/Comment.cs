using Articles.Domain.Identifiers;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Models;

public sealed class Comment
{
	public CommentId Id { get; set; }

	public CommentContent Content { get; set; }

	public UserId Author { get; set; }

	public ArticleId Article { get; set; }
}
