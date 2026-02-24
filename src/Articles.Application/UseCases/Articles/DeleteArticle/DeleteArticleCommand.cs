using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Articles.DeleteArticle;

public sealed record DeleteArticleCommand(Guid ArticleId) : ICommand;
