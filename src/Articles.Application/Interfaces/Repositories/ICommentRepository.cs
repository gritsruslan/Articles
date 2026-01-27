using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.Interfaces.Repositories;

public interface ICommentRepository
{
	Task Add(Comment comment, CancellationToken cancellationToken);

	Task<bool> Exists(CommentId commentId, CancellationToken cancellationToken);

	Task<Comment?> Get(CommentId commentId, CancellationToken cancellationToken);

	Task Delete(CommentId commentId, CancellationToken cancellationToken);

	Task<(IEnumerable<CommentReadModel> readModels, int totalCount)>
		GetReadModels(ArticleId articleId, PagedRequest pagedRequest, CancellationToken cancellationToken);
}
