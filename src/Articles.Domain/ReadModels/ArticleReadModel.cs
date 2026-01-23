namespace Articles.Domain.ReadModels;

// maybe add ViewsCount
public sealed class ArticleReadModel
{
	public Guid Id { get; set; }

	public string Title { get; set; } = null!;

	// first 200 symbols
	public string StartOfData { get; set; } = null!;

	public int BlogId { get; set; }

	public string BlogTitle { get; set; } = null!;

	public DateTime CreatedAt { get; set; }
}
