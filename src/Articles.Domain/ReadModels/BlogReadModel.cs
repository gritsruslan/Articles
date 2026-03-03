namespace Articles.Domain.ReadModels;

public class BlogReadModel
{
	public int Id { get; set; }

	public string Title { get; set; } = null!;

	public int ArticlesCount { get; set; }

	public DateTime? LastArticleCreatedAt { get; set; }
}
