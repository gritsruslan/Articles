using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public record struct UserName
{
	private UserName(string userName) => Value = userName;

	public string Value { get; }


	// Use only in cases where you are sure that the userName is valid
	public static UserName CreateVerified(string userName) => new(userName);

	public static Result<UserName> Create(string userName)
	{
		if (string.IsNullOrWhiteSpace(userName))
		{
			return UserErrors.EmptyName();
		}

		if (userName.Length is < UserConstants.NameMinLength or > UserConstants.NameMaxLength)
		{
			return UserErrors.InvalidNameLength(userName);
		}

		return new UserName(userName);
	}
}
