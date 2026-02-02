using Articles.Application.Interfaces.Repositories;

namespace Articles.Application.BlogUseCases.CreateBlog;

internal sealed class CreateBlogCommandHandler(IBlogRepository repository) : ICommandHandler<CreateBlogCommand, int>
{
	public async Task<Result<int>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
	{
		var titleResult = BlogTitle.Create(request.BlogTitle);
		if (titleResult.IsFailure)
		{
			return titleResult.Error;
		}

		var blogId = await repository.CreateBlog(titleResult.Value, cancellationToken);

		return blogId.Value;
	}
}
