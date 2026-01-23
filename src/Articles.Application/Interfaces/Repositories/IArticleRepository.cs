using Articles.Application.ArticleUseCases.GetArticles;
using Articles.Domain.ReadModels;

namespace Articles.Application.Interfaces.Repositories;

public interface IArticleRepository
{
	Task<(IEnumerable<ArticleReadModel> readModels, int totalCount)>
		GetReadModels(string? searchQuery, int skip, int take, CancellationToken cancellationToken);
}
