using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Articles.UpdateArticle;

internal sealed class UpdateArticleCommandHandler(
	IArticleRepository repository) : ICommandHandler<UpdateArticleCommand>
{
	public async Task<Result> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);

		if (!await repository.ExistsById(articleId, cancellationToken))
		{
			return ArticleErrors.NotFound(articleId);
		}

		var titleResult = ArticleTitle.Create(request.NewTitle);
		if (titleResult.IsFailure)
		{
			return titleResult.Error;
		}

		var dataResult = ArticleData.Create(request.NewData);
		if (dataResult.IsFailure)
		{
			return dataResult.Error;
		}

		await repository.Update(articleId, titleResult.Value, dataResult.Value, cancellationToken);

		return Result.Success();
	}
}
