using Articles.Application.Interfaces.Repositories;

namespace Articles.Application.ArticleUseCases.GetArticleById;

internal sealed class GetArticleByIdQueryHandler(IArticleRepository articleRepository)
	: IQueryHandler<GetArticleByIdQuery, Article>
{
	public async Task<Result<Article>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);

		var article = await articleRepository.GetById(articleId, cancellationToken);
		if (article is null)
		{
			return ArticleErrors.ArticleNotFound(articleId);
		}

		return article;
	}
}
