namespace Articles.Shared.Options;

public sealed class PermissionOptions
{
	public string[] BlogPermissions { get; init; } = [];

	public string[] ArticlePermissions { get; init; } = [];

	public string[] CommentPermissions { get; init; } = [];

	public string[] AdminPermissions { get; init; } = [];
}
