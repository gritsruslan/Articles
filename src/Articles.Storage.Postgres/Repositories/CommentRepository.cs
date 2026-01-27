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
			ArticleId = comment.ArticleId.Value
		});

		return dbContext.SaveChangesAsync(cancellationToken);
	}
}
