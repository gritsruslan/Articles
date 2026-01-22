using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;

namespace Articles.Application.BlogUseCases.GetBlogs;

internal sealed class GetBlogsQueryHandler(IBlogRepository repository) : IQueryHandler<GetBlogsQuery, IEnumerable<BlogReadModel>>
{
	public async Task<Result<IEnumerable<BlogReadModel>>>
		Handle(GetBlogsQuery request, CancellationToken cancellationToken)
	{
		var blogs = await repository.GetReadModels(cancellationToken);

		// idk why implicit
		return Result<IEnumerable<BlogReadModel>>.Success(blogs);
	}
}
