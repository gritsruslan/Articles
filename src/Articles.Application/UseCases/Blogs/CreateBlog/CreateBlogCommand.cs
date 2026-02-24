using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Blogs.CreateBlog;

public sealed record CreateBlogCommand(string BlogTitle) : ICommand<int>, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) BlogPermissions.CreateBlog;
}
