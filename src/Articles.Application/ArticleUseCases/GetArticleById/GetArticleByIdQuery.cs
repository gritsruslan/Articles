using Articles.Domain.ReadModels;

namespace Articles.Application.ArticleUseCases.GetArticleById;

public sealed record GetArticleByIdQuery(Guid ArticleId) : IQuery<Article>;
