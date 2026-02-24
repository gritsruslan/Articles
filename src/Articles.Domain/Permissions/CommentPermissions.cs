namespace Articles.Domain.Permissions;

// All comment permissions start with 103
public enum CommentPermissions
{
	CreateComment = 103_01,
	DeleteOwnComment = 103_02,
	UpdateOwnComment = 103_03
}
