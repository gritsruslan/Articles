using Articles.Domain.ReadModels;

namespace Articles.Application.Interfaces.Repositories;

public interface IBlogRepository
{
	Task<BlogId> CreateBlog(BlogTitle title, CancellationToken cancellationToken);

	Task<Blog?> GetById(BlogId id, CancellationToken cancellationToken);

	Task<(IEnumerable<BlogReadModel> readModels, int totalCount)> GetReadModels(int skip, int take, CancellationToken cancellationToken);
}
