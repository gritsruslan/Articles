namespace Articles.Domain.Constants;

public static class MetricNames
{
	public const string Login = "articles.user.login";

	public const string Registration = "articles.user.registration";

	public const string Logout = "articles.user.logout";

	public const string OutboxProcessedMessages = "articles.outbox.processed_messages";

	public const string OutboxRetryAttempts = "articles.outbox.retry_attempts";

	public const string OutboxProcessorQueueSize = "articles.outbox_processor.batch_size";

	public const string OutboxMessageProcessingDuration = "articles.outbox.message_processing_duration";
}
