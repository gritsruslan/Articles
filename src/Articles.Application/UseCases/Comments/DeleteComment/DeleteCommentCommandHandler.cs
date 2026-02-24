using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Comments.DeleteComment;

internal sealed class DeleteCommentCommandHandler(
	ICommentRepository repository,
	IApplicationUserProvider userProvider) : ICommandHandler<DeleteCommentCommand>
{
	public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
	{
		var commentId = CommentId.Create(request.CommentId);
		var userId = userProvider.CurrentUser.Id;

		var comment = await repository.GetById(commentId, cancellationToken);
		if (comment is null)
		{
			return CommentErrors.NotFound(commentId);
		}

		if (comment.AuthorId != userId)
		{
			return CommentErrors.NotAnAuthor();
		}

		await repository.DeleteById(commentId, cancellationToken);

		return Result.Success();
	}
}
