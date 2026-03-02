namespace Articles.Domain.Constants;

public static class MetricNames
{
	public const string Login = "articles.user.login";

	public const string Registration = "articles.user.registration";

	public const string Logout = "articles.user.logout";

	public const string CreateArticles = "articles.article.create";

	public const string DeleteArticle = "articles.article.delete";

	public const string CreateComment = "articles.article-comments.create";

	public const string OutboxProcessedMessages = "articles.outbox.processed-messages";

	public const string OutboxRetryAttempts = "articles.outbox.retry-attempts";

	public const string OutboxProcessorQueueSize = "articles.outbox-processor.batch-size";

	public const string OutboxMessageProcessingDuration = "articles.outbox.message-processing-duration";
}
