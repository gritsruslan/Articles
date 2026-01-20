using Articles.Domain.Identifiers;

namespace Articles.Domain.Models;

public sealed class Blog
{
	public BlogId Id { get; set; }

	public string Title { get; set; } = null!;
}
