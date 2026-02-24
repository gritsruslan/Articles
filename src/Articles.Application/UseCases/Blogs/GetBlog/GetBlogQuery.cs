using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Blogs.GetBlog;

public sealed record GetBlogQuery(int Id) : IQuery<Blog>;
