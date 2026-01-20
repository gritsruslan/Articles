namespace Articles.Storage.Postgres.Entities;

public sealed class BlogEntity
{
	public Guid Id { get; set; }

	public required string Title { get; set; }
}
