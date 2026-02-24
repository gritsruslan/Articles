using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Articles.GetArticleComments;

internal sealed class GetArticleCommentsQueryHandler(
	IArticleRepository articleRepository,
	ICommentRepository commentRepository) : IQueryHandler<GetArticleCommentsQuery, PagedData<CommentReadModel>>
{
	public async Task<Result<PagedData<CommentReadModel>>> Handle(
		GetArticleCommentsQuery request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);

		var articleExists = await articleRepository.ExistsById(articleId, cancellationToken);
		if (!articleExists)
		{
			return ArticleErrors.NotFound(articleId);
		}

		var pagedResult = PagedRequest.Create(request.Page, request.PageSize);
		if (pagedResult.IsFailure)
		{
			return pagedResult.Error;
		}

		return await commentRepository
			.GetReadModels(articleId, pagedResult.Value, cancellationToken);;
	}
}
