using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Identifiers;
using Articles.Domain.Models;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;
using Articles.Shared.DefaultServices;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Articles.Storage.Redis.Decorators;

internal sealed class CachingArticleRepositoryDecorator(
	IArticleRepository inner,
	RedisJsonCache redisJsonCache,
	IDatabase database)
	: IArticleRepository
{
	public Task Add(Article article, CancellationToken cancellationToken) =>
		inner.Add(article, cancellationToken);

	public Task<Article?> GetById(ArticleId articleId, CancellationToken cancellationToken)
	{
		var key = GenerateArticleKey(articleId);
		return redisJsonCache.CacheAsJson(
			key,
			() => inner.GetById(articleId, cancellationToken));
	}

	public Task<bool> ExistsById(ArticleId articleId, CancellationToken cancellationToken) =>
		inner.ExistsById(articleId, cancellationToken);

	public async Task DeleteById(ArticleId articleId, CancellationToken cancellationToken)
	{
		var key = GenerateArticleKey(articleId);
		await database.KeyDeleteAsync(key);
		await inner.DeleteById(articleId, cancellationToken);
	}

	public Task IncrementViewsCount(ArticleId articleId, CancellationToken cancellationToken) =>
		inner.IncrementViewsCount(articleId, cancellationToken);

	public async Task<PagedData<ArticleSearchReadModel>> GetReadModelsByBlog(BlogId blogId, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var key = GenerateReadModelsByBlogKey(blogId, pagedRequest.Page, pagedRequest.PageSize);

		return await redisJsonCache.CacheAsJson(key,
			() => inner.GetReadModelsByBlog(blogId, pagedRequest, cancellationToken),
			ReadModelsTtl);
	}

	public async Task<PagedData<ArticleSearchReadModel>> SearchReadModels(
		string searchQuery, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var normalized = SearchPatternHelper.Normalize(searchQuery);
		var key = GenerateSearchArticlesKey(normalized, pagedRequest.Page, pagedRequest.PageSize);

		return await redisJsonCache.CacheAsJsonWithCondition(key,
			pagedRequest.Page <= 3 || normalized.Length >= 3, // cache only first 3 pages
			() => inner.SearchReadModels(normalized, pagedRequest, cancellationToken),
			ArticlesTtl);
	}

	private static readonly TimeSpan ArticlesTtl = TimeSpan.FromMinutes(5);

	private static readonly TimeSpan ReadModelsTtl = TimeSpan.FromMinutes(5);

	private static RedisKey GenerateArticleKey(ArticleId articleId) =>
		$"articles.articles.{articleId.Value}";

	private static RedisKey GenerateReadModelsByBlogKey(BlogId blogId, int page, int pageSize) =>
		$"articles.articles.by-blog.{blogId.Value}.{page}.{pageSize}";

	private static RedisKey GenerateSearchArticlesKey(string searchPattern, int page, int pageSize) =>
		$"articles.articles.search.{searchPattern}.{page}.{pageSize}";
}
