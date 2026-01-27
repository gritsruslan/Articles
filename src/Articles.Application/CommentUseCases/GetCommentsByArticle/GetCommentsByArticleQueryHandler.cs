using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.CommentUseCases.GetCommentsByArticle;

internal sealed class GetCommentsByArticleQueryHandler(
	IArticleRepository articleRepository,
	ICommentRepository commentRepository) : IQueryHandler<GetCommentsByArticleQuery, PagedData<CommentReadModel>>
{
	public async Task<Result<PagedData<CommentReadModel>>> Handle(
		GetCommentsByArticleQuery request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);

		var articleExists = await articleRepository.Exists(articleId, cancellationToken);
		if (!articleExists)
		{
			return ArticleErrors.ArticleNotFound(articleId);
		}

		var pagedResult = PagedRequest.Create(request.Page, request.PageSize);
		if (pagedResult.IsFailure)
		{
			return pagedResult.Error;
		}

		var (readModels, totalCount) = await commentRepository
			.GetReadModels(articleId, pagedResult.Value, cancellationToken);

		return new PagedData<CommentReadModel>(readModels, totalCount, request.Page, request.PageSize);
	}
}
