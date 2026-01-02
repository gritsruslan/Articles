namespace Articles.Shared.Options;

public sealed class PermissionOptions
{
	public string[] ForumPermissions { get; init; } = [];

	public string[] TopicPermissions { get; init; } = [];

	public string[] CommentPermissions { get; init; } = [];

	public string[] AdminPermissions { get; init; } = [];
}
