using Articles.Domain.ReadModels;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class ArticleRepository(ArticlesDbContext dbContext) : IArticleRepository
{
	public async Task<(IEnumerable<ArticleReadModel> readModels, int totalCount)>
		GetReadModels(string? searchQuery, int skip, int take, CancellationToken cancellationToken)
	{
		//TODO допилить

		var query = dbContext.Articles.Include(a => a.Blog);

		if (searchQuery is not null)
		{
			// create safe pattern
		}

		var readModels = await query.Select(a =>
			new ArticleReadModel()
			{
				Id = a.Id,
				Title = a.Title,
				StartOfData = a.Data.Substring(0, 200),
				BlogId = a.BlogId,
				BlogTitle = a.Blog.Title,
				CreatedAt = a.CreatedAt
			})
			.Skip(skip)
			.Take(take)
			.ToListAsync(cancellationToken);

		// redo with searchQuery
		var totalCount = await dbContext.Articles.CountAsync(cancellationToken);

		return (readModels, totalCount);
	}
}
