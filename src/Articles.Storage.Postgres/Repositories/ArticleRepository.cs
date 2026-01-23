using Articles.Domain.ReadModels;
using Articles.Storage.Postgres.Helpers;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class ArticleRepository(ArticlesDbContext dbContext) : IArticleRepository
{
	public async Task<(IEnumerable<ArticleReadModel> readModels, int totalCount)>
		GetReadModels(string? searchQuery, int skip, int take, CancellationToken cancellationToken)
	{
		var query = dbContext.Articles
			.Include(a => a.Blog)
			.AsQueryable();

		string? searchPattern = null;
		if (!string.IsNullOrWhiteSpace(searchQuery))
		{
			searchPattern = LikeStatementHelper.Normalize(searchQuery);
			searchPattern = $"%{searchPattern}%";

			query = query.Where(a =>
				EF.Functions.ILike(a.Title, searchPattern, LikeStatementHelper.EscapeCharacter));
		}

		var readModels = await query.Select(a =>
			new ArticleReadModel
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

		var totalCountQuery = dbContext.Articles.AsQueryable();
		if (!string.IsNullOrWhiteSpace(searchQuery))
		{
			totalCountQuery = totalCountQuery.Where(a =>
				EF.Functions.ILike(a.Title, searchPattern!, LikeStatementHelper.EscapeCharacter));
		}
		var totalCount = await totalCountQuery.CountAsync(cancellationToken);

		return (readModels, totalCount);
	}
}
