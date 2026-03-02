using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Articles.UpdateArticle;

public sealed record UpdateArticleCommand(Guid ArticleId, string NewTitle, string NewData) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) ArticlePermissions.UpdateOwnArticle;
}
