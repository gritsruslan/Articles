using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct UserName
{
	private UserName(string userNameStr) => Value = userNameStr;

	public string Value { get; }


	// Use only in cases where you are sure that the userName is valid
	public static UserName CreateVerified(string userNameStr) => new(userNameStr);

	public static Result<UserName> Create(string userNameStr)
	{
		if (string.IsNullOrWhiteSpace(userNameStr))
		{
			return UserErrors.EmptyName(userNameStr);
		}

		if (userNameStr.Length is < UserConstants.NameMinLength or > UserConstants.NameMaxLength)
		{
			return UserErrors.InvalidNameLength(userNameStr);
		}

		return new UserName(userNameStr);
	}
}
