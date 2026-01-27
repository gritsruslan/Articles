using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.ArticleUseCases.GetArticles;

public sealed record GetArticlesQuery(string? SearchQuery, int Page, int PageSize) : IQuery<PagedData<ArticleSearchReadModel>>;
