using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Articles.GetArticle;

public sealed record GetArticleQuery(Guid ArticleId) : IQuery<Article>;
