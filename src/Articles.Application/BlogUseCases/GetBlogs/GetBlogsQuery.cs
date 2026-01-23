using Articles.Domain.ReadModels;

namespace Articles.Application.BlogUseCases.GetBlogs;

public sealed record GetBlogsQuery(int Page, int PageSize) : IQuery<IEnumerable<BlogReadModel>>;
