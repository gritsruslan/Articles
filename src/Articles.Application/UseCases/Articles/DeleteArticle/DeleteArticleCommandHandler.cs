using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.UnitOfWork;

namespace Articles.Application.UseCases.Articles.DeleteArticle;

internal sealed class DeleteArticleCommandHandler(
	IArticleRepository articleRepository,
	IFileMetadataRepository fileRepository,
	IApplicationUserProvider userProvider,
	IUnitOfWork unitOfWork) : ICommandHandler<DeleteArticleCommand>
{
	public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);

		var article = await articleRepository.GetById(articleId, cancellationToken);
		if (article is null)
		{
			return ArticleErrors.ArticleNotFound(articleId);
		}

		if (userProvider.CurrentUser.Id != article.AuthorId)
		{
			return ArticleErrors.NotAnAuthor();
		}

		await using var scope = await unitOfWork.StartScope(cancellationToken);

		await fileRepository.UnlinkFromArticle(articleId, cancellationToken);
		await articleRepository.Delete(articleId, cancellationToken);

		await scope.Commit(cancellationToken);

		return Result.Success();
	}
}
