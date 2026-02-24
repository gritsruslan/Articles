using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.Interfaces.Repositories;

public interface IArticleRepository
{
	Task Add(Article article, CancellationToken cancellationToken);

	Task<Article?> GetById(ArticleId articleId, CancellationToken cancellationToken);

	Task<bool> ExistsById(ArticleId articleId, CancellationToken cancellationToken);

	Task DeleteById(ArticleId articleId, CancellationToken cancellationToken);

	Task IncrementViewsCount(ArticleId articleId, CancellationToken cancellationToken);

	Task<PagedData<ArticleSearchReadModel>> GetReadModelsByBlog(
		BlogId blogId, PagedRequest pagedRequest, CancellationToken cancellationToken);

	Task<PagedData<ArticleSearchReadModel>>
		SearchReadModels(string searchQuery, PagedRequest pagedRequest, CancellationToken cancellationToken);
}
