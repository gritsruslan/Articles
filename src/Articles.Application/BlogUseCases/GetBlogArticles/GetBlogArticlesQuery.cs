using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.BlogUseCases.GetBlogArticles;

public sealed record GetBlogArticlesQuery(int BlogId, int Page, int PageSize)
	: IQuery<PagedData<ArticleSearchReadModel>>;
