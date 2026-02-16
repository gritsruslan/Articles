using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.BlogUseCases.GetBlogs;

public sealed record GetBlogsQuery(int Page, int PageSize) : IQuery<PagedData<BlogReadModel>>;
