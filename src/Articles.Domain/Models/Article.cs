using Articles.Domain.Identifiers;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Models;

public sealed class Article
{
	public ArticleId Id { get; set; }

	public UserId AuthorId { get; set; }

	public BlogId BlogId { get; set; }

	public ArticleTitle Title { get; set; }

	public ArticleData Data { get; set; }

	public long ViewsCount { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime? UpdatedAt { get; set; }
}


