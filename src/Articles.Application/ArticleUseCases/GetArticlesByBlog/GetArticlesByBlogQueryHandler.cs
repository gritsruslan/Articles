using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.ArticleUseCases.GetArticlesByBlog;

internal sealed class GetArticlesByBlogQueryHandler(
	IBlogRepository blogRepository,
	IArticleRepository articleRepository)
	: IQueryHandler<GetArticlesByBlogQuery, PagedData<ArticleSearchReadModel>>
{
	public async Task<Result<PagedData<ArticleSearchReadModel>>> Handle(
		GetArticlesByBlogQuery request,
		CancellationToken cancellationToken)
	{
		var blogId = BlogId.Create(request.BlogId);
		var blogExists = await blogRepository.Exists(blogId, cancellationToken);

		if (!blogExists)
		{
			return BlogErrors.BlogNotFound(blogId);
		}

		var pagedResult = PagedRequest.Create(request.Page, request.PageSize);
		if (pagedResult.IsFailure)
		{
			return pagedResult.Error;
		}

		var pagedRequest = pagedResult.Value;

		var (readModels, totalCount) = await articleRepository.GetReadModels(
			searchQuery: null,
			blogId: blogId,
			pagedRequest,
			cancellationToken);

		var pagedData = new PagedData<ArticleSearchReadModel>
			(readModels, totalCount, pagedRequest.Page, pagedRequest.PageSize);

		return pagedData;
	}
}
