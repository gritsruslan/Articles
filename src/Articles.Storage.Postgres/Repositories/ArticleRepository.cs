using Articles.Domain.ReadModels;
using Articles.Domain.ValueObjects;
using Articles.Shared.Abstraction;
using Articles.Storage.Postgres.Entities;
using Articles.Storage.Postgres.Helpers;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class ArticleRepository(ArticlesDbContext dbContext) : IArticleRepository
{
	public Task Add(Article article, CancellationToken cancellationToken)
	{
		var entity = new ArticleEntity
		{
			Id = article.Id.Value,
			Title = article.Title.Value,
			Data = article.Data.Value,
			BlogId = article.BlogId.Value,
			ViewsCount = 0,
			AuthorId = article.AuthorId.Value
		};

		dbContext.Articles.Add(entity);

		return dbContext.SaveChangesAsync(cancellationToken);
	}

	public Task<Article?> GetById(ArticleId articleId, CancellationToken cancellationToken)
	{
		return dbContext.Articles
			.Where(a => a.Id == articleId.Value)
			.Select(a => new Article
			{
				Id = ArticleId.Create(a.Id),
				AuthorId = UserId.Create(a.AuthorId),
				BlogId = BlogId.Create(a.BlogId),
				Title = ArticleTitle.CreateVerified(a.Title),
				Data = ArticleData.CreateVerified(a.Title),
				CreatedAt = a.CreatedAt,
				UpdatedAt = a.UpdatedAt
			})
			.FirstOrDefaultAsync(cancellationToken);
	}

	public Task<bool> Exists(ArticleId articleId, CancellationToken cancellationToken)
	{
		return dbContext.Articles
			.Where(a => a.Id == articleId.Value)
			.AnyAsync(cancellationToken);
	}

	public Task Delete(ArticleId articleId, CancellationToken cancellationToken)
	{
		return dbContext.Articles
			.Where(a => a.Id == articleId.Value)
			.ExecuteDeleteAsync(cancellationToken);
	}

	public Task IncrementViewsCount(ArticleId articleId, CancellationToken cancellationToken)
	{
		return dbContext.Articles.Where(a => a.Id == articleId.Value)
			.ExecuteUpdateAsync(s => s.SetProperty(a => a.ViewsCount, a => a.ViewsCount + 1), cancellationToken);
	}

	public async Task<(IEnumerable<ArticleSearchReadModel> readModels, int totalCount)>
		GetReadModels(string? searchQuery, BlogId? blogId, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var query = dbContext.Articles
			.Include(a => a.Blog)
			.Include(a => a.Author)
			.AsQueryable();

		if (blogId is not null)
		{
			query = query.Where(a => a.BlogId == ((BlogId) blogId).Value);
		}

		string? searchPattern = null;
		if (!string.IsNullOrWhiteSpace(searchQuery))
		{
			searchPattern = LikeStatementHelper.Normalize(searchQuery);
			searchPattern = $"%{searchPattern}%";

			query = query.Where(a =>
				EF.Functions.ILike(a.Title, searchPattern, LikeStatementHelper.EscapeCharacter));
		}

		var readModels = await query.Select(a =>
			new ArticleSearchReadModel
			{
				Id = a.Id,
				Title = a.Title,
				StartOfData = a.Data.Substring(0, 200),
				BlogId = a.BlogId,
				BlogTitle = a.Blog.Title,
				ViewsCount = a.ViewsCount,
				CreatedAt = a.CreatedAt,
				AuthorId = a.AuthorId,
				AuthorName = a.Author.Name
			})
			.OrderByDescending(a => a.ViewsCount)
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
