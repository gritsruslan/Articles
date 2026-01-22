namespace Articles.Application.BlogUseCases.GetBlog;

public sealed record GetBlogQuery(int Id) : IQuery<Blog>;
