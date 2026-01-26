using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.ArticleUseCases.GetArticles;

internal sealed class GetArticlesQueryHandler(IArticleRepository repository) : IQueryHandler<GetArticlesQuery, PagedData<ArticleReadModel>>
{
	public async Task<Result<PagedData<ArticleReadModel>>> Handle(
		GetArticlesQuery request, CancellationToken cancellationToken)
	{
		var paginationValidation= PagedRequest.Create(request.Page, request.PageSize);
		if (paginationValidation.IsFailure)
		{
			return paginationValidation.Error;
		}
		var pagedRequest = paginationValidation.Value;

		var (readModels, totalCount) =
			await repository.GetReadModels(request.SearchQuery, pagedRequest, cancellationToken);
		var paged = new PagedData<ArticleReadModel>(readModels, totalCount,  pagedRequest.Page, pagedRequest.PageSize);

		return paged;
	}
}
