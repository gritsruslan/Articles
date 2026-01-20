namespace Articles.Domain.Identifiers;

public readonly record struct BlogId(Guid Value)
{
	public static BlogId New() => new(Guid.NewGuid());

	public static BlogId Create(Guid id) => new(id);

	public static BlogId Parse(string idStr) => new(Guid.Parse(idStr));
}
