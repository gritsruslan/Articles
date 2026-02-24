using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Comments.UpdateComment;

internal sealed class UpdateCommentCommandHandler(
	ICommentRepository repository,
	IApplicationUserProvider userProvider) : ICommandHandler<UpdateCommentCommand>
{
	public async Task<Result> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
	{
		var commentId = CommentId.Create(request.CommentId);

		var comment = await repository.GetById(commentId, cancellationToken);
		if (comment is null)
		{
			return CommentErrors.NotFound(commentId);
		}

		if (comment.AuthorId != userProvider.CurrentUser.Id)
		{
			return CommentErrors.NotAnAuthor();
		}

		var contentResult = CommentContent.Create(request.Content);
		if (contentResult.IsFailure)
		{
			return contentResult.Error;
		}

		await repository.UpdateContent(commentId, contentResult.Value, cancellationToken);

		return Result.Success();
	}
}
