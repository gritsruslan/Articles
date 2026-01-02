namespace Articles.Domain.Permissions;

// All article permissions start with 102

public enum ArticlePermissions
{
	CreateArticle = 102_01,
	DeleteOwnArticle = 102_02,
	LikeArticle = 102_03,
	UnlikeArticle = 102_04,
}
