namespace Articles.Domain.DomainEvents;

public struct ProcessOutboxMessageResult(OutboxMessage message, DateTime processedAt, string? error = null)
{
	public OutboxMessage Message { get; set; } = message;

	public DateTime ProcessedAt { get; set; } = processedAt;

	public string? Error { get; set; } = error;
}
