using Articles.Application.ArticleUseCases.GetArticles;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.Interfaces.Repositories;

public interface IArticleRepository
{
	Task Add(Article article, CancellationToken cancellationToken);

	Task<Article?> GetById(ArticleId articleId, CancellationToken cancellationToken);

	Task<bool> Exists(ArticleId articleId, CancellationToken cancellationToken);

	Task Delete(ArticleId articleId, CancellationToken cancellationToken);

	Task IncrementViewsCount(ArticleId articleId, CancellationToken cancellationToken);

	Task<PagedData<ArticleSearchReadModel>>
		GetReadModels(string? searchQuery, BlogId? blogId, PagedRequest pagedRequest, CancellationToken cancellationToken);
}
