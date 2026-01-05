namespace Articles.Domain.DomainEvents;

public struct ProcessOutboxMessageResult(Guid id, DateTime processedAt, string? error = null)
{
	public Guid Id { get; set; }

	public DateTime ProcessedAt { get; set; }

	public string? Error { get; set; }
}
