using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.BlogUseCases.GetBlogs;

internal sealed class GetBlogsQueryHandler(IBlogRepository repository) : IQueryHandler<GetBlogsQuery, PagedData<BlogReadModel>>
{
	public async Task<Result<PagedData<BlogReadModel>>>
		Handle(GetBlogsQuery request, CancellationToken cancellationToken)
	{
		var paginationValidation = PagedRequest.Create(request.Page, request.PageSize);
		if (paginationValidation.IsFailure)
		{
			return paginationValidation.Error;
		}
		var pagedRequest = paginationValidation.Value;

		var (readModels, totalCount) = await repository.GetReadModels(pagedRequest, cancellationToken);

		var paged = new PagedData<BlogReadModel>(readModels, totalCount, request.Page, request.PageSize);

		// idk why implicit cast is not working
		return Result<PagedData<BlogReadModel>>.Success(paged);
	}
}
