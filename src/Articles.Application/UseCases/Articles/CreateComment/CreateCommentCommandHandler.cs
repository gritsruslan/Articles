using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.DefaultServices;

namespace Articles.Application.UseCases.Articles.CreateComment;

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

		var articleExists = await articleRepository.ExistsById(articleId, cancellationToken);
		if (!articleExists)
		{
			return ArticleErrors.NotFound(articleId);
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
