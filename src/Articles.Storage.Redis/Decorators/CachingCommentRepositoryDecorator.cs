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

internal sealed class CachingCommentRepositoryDecorator(
	ICommentRepository inner,
	IDatabase database) : ICommentRepository
{
	public Task Add(Comment comment, CancellationToken cancellationToken) =>
		inner.Add(comment, cancellationToken);

	public Task<bool> Exists(CommentId commentId, CancellationToken cancellationToken) =>
		inner.Exists(commentId, cancellationToken);

	private static RedisKey GenerateReadModelKey(ArticleId articleId, int page, int pageSize) =>
		$"articles.comments.byarticle.{articleId.Value}.{page}.{pageSize}";

	private static readonly TimeSpan ReadModelTtl = TimeSpan.FromMinutes(5);

	// caching here doesn't make sense because the API doesn't have a GET endpoint for comment by its id
	public Task<Comment?> Get(CommentId commentId, CancellationToken cancellationToken) =>
		inner.Get(commentId, cancellationToken);

	public Task Delete(CommentId commentId, CancellationToken cancellationToken) =>
		inner.Delete(commentId, cancellationToken);

	public Task UpdateContent(
		CommentId commentId, CommentContent content, CancellationToken cancellationToken) =>
		inner.UpdateContent(commentId, content, cancellationToken);

	public async Task<PagedData<CommentReadModel>> GetReadModels(
		ArticleId articleId, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var key = GenerateReadModelKey(articleId, pagedRequest.Page, pagedRequest.PageSize);
		string? json = await database.StringGetAsync(key);

		if (json is not null)
		{
			return JsonConvert.DeserializeObject<PagedData<CommentReadModel>>(json)!;
		}

		var readModels = await inner.GetReadModels(articleId, pagedRequest, cancellationToken);

		await database.StringSetAsync(key, JsonConvert.SerializeObject(readModels), ReadModelTtl);
		return readModels;
	}
}
