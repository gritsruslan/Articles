using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;

namespace Articles.Application.CommentUseCases.GetCommentsByArticle;

public sealed record GetCommentsByArticleQuery(Guid ArticleId, int Page, int PageSize)
	: IQuery<PagedData<CommentReadModel>>;
