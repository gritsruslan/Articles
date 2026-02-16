namespace Articles.Domain.ReadModels;

public class BlogReadModel
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public int ArticlesCount { get; set; }

	public DateTime? LastArticleCreatedAt { get; set; }
}
