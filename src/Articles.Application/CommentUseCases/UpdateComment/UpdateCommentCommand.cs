using Articles.Application.Authorization;
using Articles.Domain.Permissions;

namespace Articles.Application.CommentUseCases.UpdateComment;

public sealed record UpdateCommentCommand(Guid CommentId, string Content) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
