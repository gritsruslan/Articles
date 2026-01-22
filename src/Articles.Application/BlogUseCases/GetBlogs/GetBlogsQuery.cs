using Articles.Domain.ReadModels;

namespace Articles.Application.BlogUseCases.GetBlogs;

public sealed record GetBlogsQuery : IQuery<IEnumerable<BlogReadModel>>;
