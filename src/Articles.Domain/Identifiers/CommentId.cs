namespace Articles.Domain.Identifiers;

public readonly record struct CommentId(Guid Value)
{
	public static CommentId New() => new(Guid.NewGuid());

	public static CommentId Create(Guid id) => new(id);

	public static CommentId Parse(string idStr) => new(Guid.Parse(idStr));
}

