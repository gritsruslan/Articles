namespace Articles.Domain.Identifiers;

public readonly record struct BlogId(int Value)
{
	public static BlogId Create(int value) => new(value);
}
