using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Comments.UpdateComment;

public sealed record UpdateCommentCommand(Guid CommentId, string Content) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
