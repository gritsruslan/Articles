using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Storage.Postgres.Entities;
using Articles.Storage.Postgres.Helpers;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class ArticleRepository(ArticlesDbContext dbContext) : IArticleRepository
{
	public Task Add(Article article, CancellationToken cancellationToken)
	{
		var entity = new ArticleEntity()
		{
			Id = article.Id.Value,
			Title = article.Title.Value,
			Data = article.Data.Value,
			BlogId = article.BlogId.Value,
			AuthorId = article.AuthorId.Value
		};

		dbContext.Articles.Add(entity);

		return dbContext.SaveChangesAsync(cancellationToken);
	}

	public async Task<(IEnumerable<ArticleReadModel> readModels, int totalCount)>
		GetReadModels(string? searchQuery, PagedRequest pagedRequest, CancellationToken cancellationToken)
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
			.Skip(pagedRequest.Skip)
			.Take(pagedRequest.Take)
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
