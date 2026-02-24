using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Blogs.CreateArticle;

public sealed record CreateArticleCommand(int BlogId, string Title, string Data, string[] AttachedFiles)
	: ICommand<Guid>, IAuthorizedCommand
{
	public int RequiredPermissionId => (int)DefaultPermissions.RequireAuthorization;
}
