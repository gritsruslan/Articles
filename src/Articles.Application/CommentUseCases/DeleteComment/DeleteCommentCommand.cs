using Articles.Application.Authorization;
using Articles.Domain.Permissions;

namespace Articles.Application.CommentUseCases.DeleteComment;

public sealed record DeleteCommentCommand(Guid CommentId) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
