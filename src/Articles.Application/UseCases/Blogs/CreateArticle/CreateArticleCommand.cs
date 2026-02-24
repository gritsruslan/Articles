using Articles.Application.Authorization;
using Articles.Domain.Constants;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Blogs.CreateArticle;

public sealed record CreateArticleCommand(int BlogId, string Title, string Data, string[] AttachedFiles)
	: ICommand<Guid>, IAuthorizedCommand, IMetricsCommand, ITracedCommand
{
	public int RequiredPermissionId => (int) ArticlePermissions.CreateArticle;

	public string CounterName => MetricNames.CreateArticles;
}
