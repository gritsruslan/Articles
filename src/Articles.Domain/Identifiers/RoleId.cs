namespace Articles.Domain.Identifiers;

public readonly record struct RoleId(int Value)
{
	public static RoleId Create(int value) => new(value);
}
