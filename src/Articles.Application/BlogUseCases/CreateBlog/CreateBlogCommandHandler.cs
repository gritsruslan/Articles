using Articles.Application.Interfaces.Repositories;

namespace Articles.Application.BlogUseCases.CreateBlog;

internal sealed class CreateBlogCommandHandler(IBlogRepository repository) : ICommandHandler<CreateBlogCommand>
{
	public async Task<Result> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
	{
		var titleResult = BlogTitle.Create(request.BlogTitle);
		if (titleResult.IsFailure)
		{
			return titleResult.Error;
		}

		await repository.CreateBlog(titleResult.Value, cancellationToken);

		return Result.Success();
	}
}
