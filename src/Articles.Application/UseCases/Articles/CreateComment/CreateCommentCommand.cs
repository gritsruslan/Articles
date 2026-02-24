using Articles.Application.Authorization;
using Articles.Domain.Constants;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Articles.CreateComment;

public sealed record CreateCommentCommand(Guid ArticleId, string Content) :
	ICommand<Comment>, IAuthorizedCommand, IMetricsCommand, ITracedCommand
{
	public int RequiredPermissionId => (int) CommentPermissions.CreateComment;

	public string CounterName => MetricNames.CreateComment;
}
