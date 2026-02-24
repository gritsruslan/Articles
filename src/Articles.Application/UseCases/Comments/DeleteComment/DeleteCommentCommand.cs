using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Comments.DeleteComment;

public sealed record DeleteCommentCommand(Guid CommentId) : ICommand, IAuthorizedCommand, ITracedCommand
{
	public int RequiredPermissionId => (int) CommentPermissions.DeleteOwnComment;
}
