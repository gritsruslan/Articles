namespace Articles.Shared.Options;

public sealed class PermissionOptions
{
	public string[] BlogPermissions { get; set; } = [];

	public string[] ArticlePermissions { get; set; } = [];

	public string[] CommentPermissions { get; set; } = [];

	public string[] AdminPermissions { get; set; } = [];
}
