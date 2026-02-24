using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Articles.GetArticles;

public sealed record GetArticlesQuery(string SearchQuery, int Page, int PageSize) : IQuery<PagedData<ArticleSearchReadModel>>;
