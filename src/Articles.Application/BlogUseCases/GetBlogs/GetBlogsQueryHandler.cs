using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;

namespace Articles.Application.BlogUseCases.GetBlogs;

internal sealed class GetBlogsQueryHandler(IBlogRepository repository) : IQueryHandler<GetBlogsQuery, IEnumerable<BlogReadModel>>
{
	public async Task<Result<IEnumerable<BlogReadModel>>>
		Handle(GetBlogsQuery request, CancellationToken cancellationToken)
	{
		int skip = (request.Page - 1) * request.PageSize;
		int take = request.PageSize;

		var blogs = await repository.GetReadModels(skip, take, cancellationToken);

		// idk why implicit cast is not working
		return Result<IEnumerable<BlogReadModel>>.Success(blogs);
	}
}
