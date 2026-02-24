using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Articles.GetArticle;

internal sealed class GetArticleQueryHandler(IArticleRepository articleRepository)
	: IQueryHandler<GetArticleQuery, Article>
{
	public async Task<Result<Article>> Handle(GetArticleQuery request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);

		var article = await articleRepository.GetById(articleId, cancellationToken);
		if (article is null)
		{
			return ArticleErrors.NotFound(articleId);
		}

		await articleRepository.IncrementViewsCount(articleId, cancellationToken);

		return article;
	}
}
