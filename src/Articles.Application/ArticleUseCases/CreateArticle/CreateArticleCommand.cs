using Articles.Application.Authorization;
using Articles.Domain.Permissions;

namespace Articles.Application.ArticleUseCases.CreateArticle;

public sealed record CreateArticleCommand(int BlogId, string Title, string Data, string[] AttachedFiles)
	: ICommand<Guid>, IAuthorizedCommand
{
	public int RequiredPermissionId => (int)DefaultPermissions.RequireAuthorization;
}
