using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Articles.CreateComment;

public sealed record CreateCommentCommand(Guid ArticleId, string Content) : ICommand<Comment>, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
