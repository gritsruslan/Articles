using Articles.Domain.ReadModels;
using Articles.Domain.ValueObjects;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;
using Articles.Storage.Postgres.Entities;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class CommentRepository(ArticlesDbContext dbContext) : ICommentRepository
{
	public Task Add(Comment comment, CancellationToken cancellationToken)
	{
		dbContext.Comments.Add(new CommentEntity
		{
			Id = comment.Id.Value,
			Content = comment.Content.Value,
			AuthorId = comment.AuthorId.Value,
			ArticleId = comment.ArticleId.Value,
			CreatedAt = comment.CreatedAt
		});

		return dbContext.SaveChangesAsync(cancellationToken);
	}

	public Task<bool> ExistsById(CommentId commentId, CancellationToken cancellationToken)
	{
		return dbContext.Comments
			.AnyAsync(c => c.Id == commentId.Value, cancellationToken);
	}

	public Task<Comment?> GetById(CommentId commentId, CancellationToken cancellationToken)
	{
		return dbContext.Comments
			.Where(c => c.Id == commentId.Value)
			.Select(c => new Comment
			{
				Id = CommentId.Create(c.Id),
				Content = CommentContent.CreateVerified(c.Content),
				AuthorId = UserId.Create(c.AuthorId),
				ArticleId = ArticleId.Create(c.ArticleId),
				CreatedAt = c.CreatedAt,
				UpdatedAt = c.UpdatedAt
			})
			.FirstOrDefaultAsync(cancellationToken);
	}

	public Task DeleteById(CommentId commentId, CancellationToken cancellationToken)
	{
		return dbContext.Comments
			.Where(c => c.Id == commentId.Value)
			.ExecuteDeleteAsync(cancellationToken);
	}

	public Task UpdateContent(CommentId commentId, CommentContent content, CancellationToken cancellationToken)
	{
		return dbContext.Comments
			.Where(c => c.Id == commentId.Value)
			.ExecuteUpdateAsync(s =>
				s.SetProperty(c => c.Content, content.Value), cancellationToken);
	}

	public async Task<PagedData<CommentReadModel>>
		GetReadModels(ArticleId articleId, PagedRequest pagedRequest, CancellationToken cancellationToken)
	{
		var readModels = await dbContext.Comments
			.Include(a => a.Author)
			.Where(c => c.ArticleId == articleId.Value)
			.Skip(pagedRequest.Skip)
			.Take(pagedRequest.Take)
			.Select(c => new CommentReadModel
			{
				Id = c.Id,
				Content = c.Content,
				AuthorId = c.AuthorId,
				AuthorName = c.Author.Name,
				CreatedAt = c.CreatedAt
			}).ToListAsync(cancellationToken);

		var totalCount = await dbContext.Comments
			.CountAsync(c => c.ArticleId == articleId.Value, cancellationToken);

		return new PagedData<CommentReadModel>(readModels, totalCount, pagedRequest.Page, pagedRequest.PageSize);
	}
}
