using Articles.Domain.ReadModels;
using Articles.Shared.Abstraction;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Abstraction.Pagination;

namespace Articles.Application.UseCases.Articles.GetArticleComments;

public sealed record GetArticleCommentsQuery(Guid ArticleId, int Page, int PageSize)
	: IQuery<PagedData<CommentReadModel>>;
