using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Identifiers;
using Articles.Domain.Models;
using Articles.Domain.ReadModels;
using Articles.Domain.ValueObjects;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Articles.Storage.Redis.Decorators;

internal sealed class CachingBlogRepositoryDecorator(
	IBlogRepository inner,
	RedisJsonCache redisJsonCache) : IBlogRepository
{
	public Task<BlogId> CreateBlog(BlogTitle title, CancellationToken cancellationToken) =>
		inner.CreateBlog(title, cancellationToken);

	public async Task<Blog?> GetById(BlogId id, CancellationToken cancellationToken)
	{
		var key = GenerateBlogKey(id);
		return await redisJsonCache.CacheAsJson(
			key,
			() => inner.GetById(id, cancellationToken),
			BlogsTtl);
	}

	public Task<bool> ExistsById(BlogId id, CancellationToken cancellationToken) =>
		inner.ExistsById(id, cancellationToken);

	public async Task<PagedData<BlogReadModel>>
		GetReadModels(PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var key = GenerateReadModelsKey(pagedRequest.Page, pagedRequest.PageSize);
		return await redisJsonCache.CacheAsJsonWithCondition(
			key,
			pagedRequest.Page <= 10, // cache only the first 10 pages
			() => inner.GetReadModels(pagedRequest, cancellationToken),
			ReadModelsTtl);
	}

	private static readonly TimeSpan ReadModelsTtl = TimeSpan.FromSeconds(30);

	private static readonly TimeSpan BlogsTtl = TimeSpan.FromSeconds(30);

	private static RedisKey GenerateBlogKey(BlogId blogId) =>
		$"articles.blogs.{blogId.Value}";

	private static RedisKey GenerateReadModelsKey(int page, int pageSize) =>
		$"articles.blogs.search.page.{page}.pageSize.{pageSize}";
}
