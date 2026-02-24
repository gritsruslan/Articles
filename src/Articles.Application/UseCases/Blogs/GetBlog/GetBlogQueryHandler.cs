using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Blogs.GetBlog;

internal sealed class GetBlogQueryHandler(IBlogRepository repository) : IQueryHandler<GetBlogQuery, Blog>
{
	public async Task<Result<Blog>> Handle(GetBlogQuery request, CancellationToken cancellationToken)
	{
		var blogId = BlogId.Create(request.Id);
		var blog = await repository.GetById(blogId, cancellationToken);

		if (blog is null)
		{
			return BlogErrors.BlogNotFound(blogId);
		}

		return blog;
	}
}
