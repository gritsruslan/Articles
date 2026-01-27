using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;

namespace Articles.Application.CommentUseCases.CreateComment;

internal sealed class CreateCommentCommandHandler(
	IArticleRepository articleRepository,
	ICommentRepository commentRepository,
	IApplicationUserProvider userProvider) : ICommandHandler<CreateCommentCommand>
{
	public async Task<Result> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
	{
		var articleId = ArticleId.Create(request.ArticleId);
		var newCommentId = CommentId.New();

		var articleExists = await articleRepository.Exists(articleId, cancellationToken);
		if (!articleExists)
		{
			return ArticleErrors.ArticleNotFound(articleId);
		}

		var contentResult = CommentContent.Create(request.Content);
		if (contentResult.IsFailure)
		{
			return contentResult.Error;
		}

		var comment = new Comment
		{
			Id = newCommentId,
			ArticleId = articleId,
			AuthorId = userProvider.CurrentUser.Id,
			Content = contentResult.Value
		};

		await commentRepository.Add(comment, cancellationToken);

		return Result.Success();
	}
}
