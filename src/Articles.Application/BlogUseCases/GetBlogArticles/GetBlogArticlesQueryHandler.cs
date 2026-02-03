using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.BlogUseCases.GetBlogArticles;

internal sealed class GetBlogArticlesQueryHandler(
	IBlogRepository blogRepository,
	IArticleRepository articleRepository) :
	IQueryHandler<GetBlogArticlesQuery, PagedData<ArticleSearchReadModel>>
{
	public async Task<Result<PagedData<ArticleSearchReadModel>>> Handle(
		GetBlogArticlesQuery request,
		CancellationToken cancellationToken)
	{
		var blogId = BlogId.Create(request.BlogId);

		var createPagedRequestResult = PagedRequest.Create(request.Page, request.PageSize);
		if (createPagedRequestResult.IsFailure)
		{
			return createPagedRequestResult.Error;
		}

		var exists = await blogRepository.Exists(blogId, cancellationToken);
		if (!exists)
		{
			return BlogErrors.BlogNotFound(blogId);
		}

		var pagedRequest = createPagedRequestResult.Value;
		var (articles, totalCount) =
			await articleRepository.GetReadModels(searchQuery: null, blogId, pagedRequest, cancellationToken);

		return new PagedData<ArticleSearchReadModel>(articles, totalCount, request.Page, request.PageSize);
	}
}
