namespace Articles.Domain.Permissions;

// All article permissions begin with 102
public enum ArticlePermissions
{
	CreateArticle = 102_01,
	DeleteOwnArticle = 102_02,
	UpdateOwnArticle = 102_03
}
