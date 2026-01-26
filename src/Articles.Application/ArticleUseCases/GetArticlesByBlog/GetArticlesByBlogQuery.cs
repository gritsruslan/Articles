using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.ArticleUseCases.GetArticlesByBlog;

public sealed record GetArticlesByBlogQuery(int BlogId, int Page, int PageSize)
	: IQuery<PagedData<ArticleReadModel>>;
