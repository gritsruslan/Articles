using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Articles.GetArticles;

internal sealed class GetArticlesQueryHandler(IArticleRepository repository) : IQueryHandler<GetArticlesQuery, PagedData<ArticleSearchReadModel>>
{
	public async Task<Result<PagedData<ArticleSearchReadModel>>> Handle(
		GetArticlesQuery request, CancellationToken cancellationToken)
	{
		// TODO : validate searchQuery
		var paginationValidation= PagedRequest.Create(request.Page, request.PageSize);
		if (paginationValidation.IsFailure)
		{
			return paginationValidation.Error;
		}
		var pagedRequest = paginationValidation.Value;

		return await repository.SearchReadModels(request.SearchQuery, pagedRequest, cancellationToken);
	}
}
