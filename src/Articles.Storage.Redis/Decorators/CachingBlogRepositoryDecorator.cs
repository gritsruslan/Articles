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
	IDatabase database) : IBlogRepository
{
	private static RedisKey GenerateBlogKey(BlogId blogId) =>
		$"articles.blogs.{blogId.Value}";

	private static RedisKey GenerateReadModelsKey(int page, int pageSize) =>
		$"articles.blogs.search.page.{page}.pageSize.{pageSize}";

	private static readonly TimeSpan ReadModelsTtl = TimeSpan.FromSeconds(30);

	private static readonly TimeSpan BlogsTtl = TimeSpan.FromSeconds(30);

	public Task<BlogId> CreateBlog(BlogTitle title, CancellationToken cancellationToken) =>
		inner.CreateBlog(title, cancellationToken);

	public async Task<Blog?> GetById(BlogId id, CancellationToken cancellationToken)
	{
		var key = GenerateBlogKey(id);

		string? blogJson = await database.StringGetAsync(key);
		if (blogJson is not null)
		{
			return JsonConvert.DeserializeObject<Blog>(blogJson);
		}

		var blog = await inner.GetById(id, cancellationToken);
		if (blog is not null)
		{
			blogJson = JsonConvert.SerializeObject(blog);
			await database.StringSetAsync(key, blogJson, BlogsTtl);
		}

		return blog;
	}

	public Task<bool> Exists(BlogId id, CancellationToken cancellationToken) =>
		inner.Exists(id, cancellationToken);

	public async Task<PagedData<BlogReadModel>>
		GetReadModels(PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var key = GenerateReadModelsKey(pagedRequest.Page, pagedRequest.PageSize);

		if (pagedRequest.Page <= 10)
		{
			string? json = await database.StringGetAsync(key);
			if (json is not null)
			{
				return JsonConvert.DeserializeObject<PagedData<BlogReadModel>>(json)!;
			}
		}

		var readModels = await inner.GetReadModels(pagedRequest, cancellationToken);

		// cache only the first 10 pages
		if (pagedRequest.Page <= 10)
		{
			await database.StringSetAsync(key, JsonConvert.SerializeObject(readModels), ReadModelsTtl);
		}

		return readModels;
	}
}
