using Articles.Application.Authorization;
using Articles.Domain.Permissions;

namespace Articles.Application.CommentUseCases.CreateComment;

public sealed record CreateCommentCommand(Guid ArticleId, string Content) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
