namespace Articles.UnitTests.ValueObjects;

public class UserNameUnitTests
{
	[Theory]
	[InlineData("d")]
	[InlineData("Dima")]
	[InlineData("5252")]
	[InlineData("1488hello")]
	public void CreateUserName_WhenItIsValid(string userNameStr)
	{
		var result = UserName.Create(userNameStr);
		result.IsSuccess.Should().BeTrue();
	}

	[Theory]
	[MemberData(nameof(GetInvalidUsernames))]
	public void DontCreateUserName_WhenItIsNotValid(string userNameStr)
	{
		var result = UserName.Create(userNameStr);
		result.IsFailure.Should().BeTrue();
	}

	public static IEnumerable<object[]> GetInvalidUsernames()
	{
		yield return [string.Empty];
		yield return [string.Join(string.Empty, Enumerable.Range(0, 50 + 1).Select(_ => "A"))];
	}
}
