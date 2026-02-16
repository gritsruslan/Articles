namespace Articles.Domain.ReadModels;

// maybe add ViewsCount
public sealed class ArticleSearchReadModel
{
	public Guid Id { get; set; }

	public string Title { get; set; } = null!;

	// first 200 symbols
	public string StartOfData { get; set; } = null!;

	public int BlogId { get; set; }

	public string BlogTitle { get; set; } = null!;

	public Guid AuthorId { get; set; }

	public string AuthorName { get; set; }

	public long ViewsCount { get; set; }

	public DateTime CreatedAt { get; set; }
}
