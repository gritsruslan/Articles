using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.Interfaces.Repositories;

public interface IBlogRepository
{
	Task<BlogId> CreateBlog(BlogTitle title, CancellationToken cancellationToken);

	Task<Blog?> GetById(BlogId id, CancellationToken cancellationToken);

	Task<bool> Exists(BlogId id, CancellationToken cancellationToken);

	Task<PagedData<BlogReadModel>> GetReadModels(PagedRequest pagedRequest, CancellationToken cancellationToken);
}
