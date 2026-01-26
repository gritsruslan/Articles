namespace Articles.Application.ArticleUseCases.DeleteArticle;

public sealed record DeleteArticleCommand(Guid ArticleId) : ICommand;
