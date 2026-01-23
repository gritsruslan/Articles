using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.BlogUseCases.GetBlogs;

internal sealed class GetBlogsQueryHandler(IBlogRepository repository) : IQueryHandler<GetBlogsQuery, PagedData<BlogReadModel>>
{
	public async Task<Result<PagedData<BlogReadModel>>>
		Handle(GetBlogsQuery request, CancellationToken cancellationToken)
	{
		// TODO validate page and pageSize
		int skip = (request.Page - 1) * request.PageSize;
		int take = request.PageSize;

		var (readModels, totalCount) = await repository.GetReadModels(skip, take, cancellationToken);

		var paged = new PagedData<BlogReadModel>(readModels, totalCount, request.Page, request.PageSize);

		// idk why implicit cast is not working
		return Result<PagedData<BlogReadModel>>.Success(paged);
	}
}
