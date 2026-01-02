namespace Articles.Domain.Permissions;

// add admin permissions starts with 999
public enum AdminPermissions
{
	BlockUser = 999_01,
	DeleteTopic = 999_02,
	DeleteComment = 999_03
}
