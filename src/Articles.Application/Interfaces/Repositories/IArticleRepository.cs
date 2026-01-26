using Articles.Application.ArticleUseCases.GetArticles;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.Interfaces.Repositories;

public interface IArticleRepository
{
	Task Add(Article article, CancellationToken cancellationToken);

	Task<(IEnumerable<ArticleReadModel> readModels, int totalCount)>
		GetReadModels(string? searchQuery, PagedRequest pagedRequest, CancellationToken cancellationToken);
}
