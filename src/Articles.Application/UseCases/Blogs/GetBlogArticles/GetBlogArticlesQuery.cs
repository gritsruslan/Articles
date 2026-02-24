using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Blogs.GetBlogArticles;

public sealed record GetBlogArticlesQuery(int BlogId, int Page, int PageSize)
	: IQuery<PagedData<ArticleSearchReadModel>>;
