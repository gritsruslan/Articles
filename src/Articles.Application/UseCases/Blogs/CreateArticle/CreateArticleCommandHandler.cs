using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.DefaultServices;
using Articles.Shared.UnitOfWork;

namespace Articles.Application.UseCases.Blogs.CreateArticle;

internal sealed class CreateArticleCommandHandler(
	IArticleRepository articleRepository,
	IFileMetadataRepository fileRepository,
	IBlogRepository blogRepository,
	IApplicationUserProvider userProvider,
	IDateTimeProvider dateTimeProvider,
	IUnitOfWork unitOfWork)
	: ICommandHandler<CreateArticleCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
	{
		var titleResult = ArticleTitle.Create(request.Title);
		if (titleResult.IsFailure)
		{
			return titleResult.Error;
		}

		var dataResult = ArticleData.Create(request.Data);
		if (dataResult.IsFailure)
		{
			return dataResult.Error;
		}

		UserId userId = userProvider.CurrentUser.Id;
		BlogId blogId = BlogId.Create(request.BlogId);
		ArticleId articleId = ArticleId.New();

		var blogExists = await blogRepository.Exists(blogId, cancellationToken);
		if (!blogExists)
		{
			return BlogErrors.BlogNotFound(blogId);
		}

		var article = new Article
		{
			Id = articleId,
			AuthorId = userId,
			BlogId = blogId,
			Title = titleResult.Value,
			Data = dataResult.Value,
			CreatedAt = dateTimeProvider.UtcNow
		};

		List<Guid> fileIds = new List<Guid>(request.AttachedFiles.Length);
		foreach (var fileName in request.AttachedFiles)
		{
			var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (!Guid.TryParse(nameWithoutExtension, out var fileId))
			{
				return FileErrors.FileNotFound(fileName);
			}
			fileIds.Add(fileId);
		}

		await using var scope = await unitOfWork.StartScope(cancellationToken);

		await articleRepository.Add(article, cancellationToken);
		await fileRepository.LinkToArticle(fileIds, articleId, cancellationToken);

		await scope.Commit(cancellationToken);

		return articleId.Value;
	}
}
