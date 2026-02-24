using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Blogs.GetBlogs;

public sealed record GetBlogsQuery(int Page, int PageSize) : IQuery<PagedData<BlogReadModel>>;
