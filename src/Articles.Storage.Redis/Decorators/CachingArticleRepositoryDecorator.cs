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
	IArticleRepository inner, IDatabase database)
	: IArticleRepository
{
	public Task Add(Article article, CancellationToken cancellationToken) =>
		inner.Add(article, cancellationToken);

	public async Task<Article?> GetById(ArticleId articleId, CancellationToken cancellationToken)
	{
		var key = GenerateArticleKey(articleId);

		string? articleJson = await database.StringGetAsync(key);
		if (articleJson is not null)
		{
			return JsonConvert.DeserializeObject<Article>(articleJson);
		}

		var article = await inner.GetById(articleId, cancellationToken);
		if (article is not null)
		{
			var json = JsonConvert.SerializeObject(article);
			await database.StringSetAsync(key, json, ArticlesTtl);
		}

		return article;
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

		string? readModelsJson = await database.StringGetAsync(key);
		if (readModelsJson is not null)
		{
			return JsonConvert.DeserializeObject<PagedData<ArticleSearchReadModel>>(readModelsJson)!;
		}

		var readModels = await inner.GetReadModelsByBlog(blogId, pagedRequest, cancellationToken);
		await database.StringSetAsync(key, JsonConvert.SerializeObject(readModels), ReadModelsTtl);

		return readModels;
	}

	public async Task<PagedData<ArticleSearchReadModel>> SearchReadModels(
		string searchQuery, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var normalized = SearchPatternHelper.Normalize(searchQuery);

		// cache only first 3 pages
		if (pagedRequest.Page >= 3 || normalized.Length < 3)
		{
			return await inner.SearchReadModels(searchQuery, pagedRequest, cancellationToken);
		}

		var key = GenerateSearchArticlesKey(normalized, pagedRequest.Page, pagedRequest.PageSize);

		string? json = await database.StringGetAsync(key);
		if (json is not null)
		{
			return JsonConvert.DeserializeObject<PagedData<ArticleSearchReadModel>>(json)!;
		}

		var readModels = await inner.SearchReadModels(searchQuery, pagedRequest, cancellationToken);

		await database.StringSetAsync(key, JsonConvert.SerializeObject(readModels), ReadModelsTtl);

		return readModels;
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
