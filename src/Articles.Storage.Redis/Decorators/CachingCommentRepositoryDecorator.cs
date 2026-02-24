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
	RedisJsonCache redisJsonCache,
	IDatabase database) : ICommentRepository
{
	public Task Add(Comment comment, CancellationToken cancellationToken) =>
		inner.Add(comment, cancellationToken);

	public Task<bool> ExistsById(CommentId commentId, CancellationToken cancellationToken) =>
		inner.ExistsById(commentId, cancellationToken);

	// caching here doesn't make sense because the API doesn't have a GET endpoint for comment by its id
	public Task<Comment?> GetById(CommentId commentId, CancellationToken cancellationToken) =>
		inner.GetById(commentId, cancellationToken);

	public Task DeleteById(CommentId commentId, CancellationToken cancellationToken) =>
		inner.DeleteById(commentId, cancellationToken);

	public Task UpdateContent(
		CommentId commentId, CommentContent content, CancellationToken cancellationToken) =>
		inner.UpdateContent(commentId, content, cancellationToken);

	public async Task<PagedData<CommentReadModel>> GetReadModels(
		ArticleId articleId, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var key = GenerateReadModelKey(articleId, pagedRequest.Page, pagedRequest.PageSize);
		return await redisJsonCache.CacheAsJsonWithCondition(
			key,
			pagedRequest.Page <= 10,
			() => inner.GetReadModels(articleId, pagedRequest, cancellationToken),
			ReadModelTtl);
	}

	private static readonly TimeSpan ReadModelTtl = TimeSpan.FromMinutes(5);

	private static RedisKey GenerateReadModelKey(ArticleId articleId, int page, int pageSize) =>
		$"articles.comments.by-article.{articleId.Value}.{page}.{pageSize}";

}
