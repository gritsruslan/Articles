namespace Articles.Domain.Identifiers;

public readonly record struct SessionId(Guid Value)
{
	public static SessionId New() => new(Guid.NewGuid());

	public static SessionId Create(Guid id) => new(id);

	public static SessionId Parse(string idStr) => new(Guid.Parse(idStr));
}
