using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
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

	public async Task<(IEnumerable<CommentReadModel> readModels, int totalCount)>
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
			.Where(c => c.ArticleId == articleId.Value)
			.CountAsync(cancellationToken);

		return (readModels, totalCount);
	}
}
