namespace Articles.Domain.Identifiers;

public readonly record struct UserId(Guid Value)
{
	public static UserId Empty => new(Guid.Empty);

	public static UserId New() => new(Guid.NewGuid());

	public static UserId Create(Guid id) => new(id);

	public static UserId Parse(string idStr) => new(Guid.Parse(idStr));
}
