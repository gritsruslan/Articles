namespace Articles.Domain.Permissions;

// All topic permissions start with 102

public enum TopicPermissions
{
	CreateTopic = 102_01,
	DeleteOwnTopic = 102_02,
	LikeTopic = 102_03,
	UnlikeTopic = 102_04,
}
