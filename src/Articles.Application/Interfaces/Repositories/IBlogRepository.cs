using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.Interfaces.Repositories;

public interface IBlogRepository
{
	Task<BlogId> CreateBlog(BlogTitle title, CancellationToken cancellationToken);

	Task<Blog?> GetById(BlogId id, CancellationToken cancellationToken);

	Task<(IEnumerable<BlogReadModel> readModels, int totalCount)> GetReadModels(PagedRequest pagedRequest, CancellationToken cancellationToken);
}
