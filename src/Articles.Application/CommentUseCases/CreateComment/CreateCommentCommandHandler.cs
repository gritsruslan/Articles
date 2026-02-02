using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.DefaultServices;

namespace Articles.Application.CommentUseCases.CreateComment;

internal sealed class CreateCommentCommandHandler(
	IArticleRepository articleRepository,
	ICommentRepository commentRepository,
	IDateTimeProvider dateTimeProvider,
	IApplicationUserProvider userProvider) : ICommandHandler<CreateCommentCommand, Comment>
{
	public async Task<Result<Comment>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
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
			Content = contentResult.Value,
			CreatedAt = dateTimeProvider.UtcNow
		};

		await commentRepository.Add(comment, cancellationToken);

		return comment;
	}
}
