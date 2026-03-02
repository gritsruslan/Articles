namespace Articles.Domain.Permissions;

// All comment permissions begin with 103
public enum CommentPermissions
{
	CreateComment = 103_01,
	DeleteOwnComment = 103_02,
	UpdateOwnComment = 103_03
}
