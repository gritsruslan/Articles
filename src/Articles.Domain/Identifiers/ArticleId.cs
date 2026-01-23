namespace Articles.Domain.Identifiers;

public readonly record struct ArticleId(Guid Value)
{
	public static ArticleId New() => new(Guid.NewGuid());

	public static ArticleId Create(Guid id) => new(id);

	public static ArticleId Parse(string idStr) => new(Guid.Parse(idStr));
}

