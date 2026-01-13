namespace Articles.Domain.DomainEvents;

public sealed class OutboxMessage
{
	public Guid Id { get; set; }

	public DateTime EmittedAt { get; set; }

	public DateTime? ProcessedAt { get; set; }

	public string Type { get; set; } = null!;

	public string ContentBlob { get; set; } = null!;

	public string? TraceId { get; set; }

	public string? SpanId { get; set; }

	public string? Error { get; set; }
}
