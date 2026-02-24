using Articles.Application.Interfaces.Repositories;
using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Blogs.GetBlogs;

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

		return await repository.GetReadModels(pagedRequest, cancellationToken);;
	}
}
