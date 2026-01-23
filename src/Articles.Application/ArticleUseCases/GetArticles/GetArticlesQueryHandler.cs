using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.ArticleUseCases.GetArticles;

internal sealed class GetArticlesQueryHandler(IArticleRepository repository) : IQueryHandler<GetArticlesQuery, PagedData<ArticleReadModel>>
{
	public async Task<Result<PagedData<ArticleReadModel>>> Handle(
		GetArticlesQuery request, CancellationToken cancellationToken)
	{
		var page = request.Page;
		var pageSize = request.PageSize;
		var skip = (page - 1) * pageSize;
		var take = pageSize;

		var (readModels, totalCount) =
			await repository.GetReadModels(request.SearchQuery, skip, take, cancellationToken);
		var paged = new PagedData<ArticleReadModel>(readModels, totalCount, page, pageSize);

		return paged;
	}
}
