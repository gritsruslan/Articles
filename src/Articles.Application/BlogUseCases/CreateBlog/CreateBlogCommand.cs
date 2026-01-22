using Articles.Application.Authorization;
using Articles.Domain.Permissions;

namespace Articles.Application.BlogUseCases.CreateBlog;

public sealed record CreateBlogCommand(string BlogTitle) : ICommand, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) BlogPermissions.CreateBlog;
}
