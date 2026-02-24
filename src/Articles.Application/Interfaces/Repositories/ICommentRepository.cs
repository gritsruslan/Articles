using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.Interfaces.Repositories;

public interface ICommentRepository
{
	Task Add(Comment comment, CancellationToken cancellationToken);

	Task<bool> Exists(CommentId commentId, CancellationToken cancellationToken);

	Task<Comment?> Get(CommentId commentId, CancellationToken cancellationToken);

	Task Delete(CommentId commentId, CancellationToken cancellationToken);

	Task UpdateContent(CommentId commentId, CommentContent content, CancellationToken cancellationToken);

	Task<PagedData<CommentReadModel>> GetReadModels(
		ArticleId articleId, PagedRequest pagedRequest, CancellationToken cancellationToken);
}
