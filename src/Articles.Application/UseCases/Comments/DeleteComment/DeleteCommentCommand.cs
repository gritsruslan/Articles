using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Comments.DeleteComment;

public sealed record DeleteCommentCommand(Guid CommentId) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
