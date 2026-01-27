using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;

namespace Articles.Application.CommentUseCases.DeleteComment;

internal sealed class DeleteCommentCommandHandler(
	ICommentRepository repository,
	IApplicationUserProvider userProvider) : ICommandHandler<DeleteCommentCommand>
{
	public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
	{
		var commentId = CommentId.Create(request.CommentId);
		var userId = userProvider.CurrentUser.Id;

		var comment = await repository.Get(commentId, cancellationToken);
		if (comment is null)
		{
			return CommentErrors.NotFound(commentId);
		}

		if (comment.AuthorId != userId)
		{
			return CommentErrors.NotAnAuthor();
		}

		await repository.Delete(commentId, cancellationToken);

		return Result.Success();
	}
}
