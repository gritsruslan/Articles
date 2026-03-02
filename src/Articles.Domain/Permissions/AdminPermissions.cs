namespace Articles.Domain.Permissions;

// add admin permissions begin with 999
// for future
public enum AdminPermissions
{
	BlockUser = 999_01,
	DeleteArticle = 999_02,
	DeleteComment = 999_03
}
