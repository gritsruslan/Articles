using Articles.Domain.Identifiers;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Models;

public sealed class Blog
{
	public BlogId Id { get; set; }

	public BlogTitle Title { get; set; }
}
