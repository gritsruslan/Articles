using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Constants;
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
		const int searchQueryMinLength = 2;
		const int searchQueryMaxLength = ArticleConstants.TitleMaxLength;
		var searchQuery = request.SearchQuery;

		if (searchQuery.Length < searchQueryMinLength || searchQuery.Length > searchQueryMaxLength)
		{
			return AbstractErrors.InvalidParameterLength(
				nameof(searchQuery),
				searchQuery,
				2,
				ArticleConstants.TitleMaxLength);
		}

		var paginationValidation= PagedRequest.Create(request.Page, request.PageSize);
		if (paginationValidation.IsFailure)
		{
			return paginationValidation.Error;
		}
		var pagedRequest = paginationValidation.Value;

		return await repository.SearchReadModels(searchQuery, pagedRequest, cancellationToken);
	}
}
