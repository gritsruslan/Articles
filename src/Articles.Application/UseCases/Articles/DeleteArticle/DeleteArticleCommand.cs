using Articles.Application.Authorization;
using Articles.Domain.Constants;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Articles.DeleteArticle;

public sealed record DeleteArticleCommand(Guid ArticleId) : ICommand, IAuthorizedCommand, IMetricsCommand, ITracedCommand
{
	public int RequiredPermissionId => (int)ArticlePermissions.DeleteOwnArticle;
	
	public string CounterName => MetricNames.DeleteArticle;
}
