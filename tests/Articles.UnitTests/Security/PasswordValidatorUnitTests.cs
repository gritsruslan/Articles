using Articles.Infrastructure.Security;

namespace Articles.UnitTests.Security;

public class PasswordValidatorUnitTests
{
	private readonly IPasswordValidator _validator = new PasswordValidator();

	[Theory]
	[InlineData("Password123")]
	[InlineData("1_p222P_2")]
	[InlineData("Hello,1,World")]
	public void ReturnSuccess_WhenPasswordIsValid(string password)
	{
		var result = _validator.Validate(password);
		result.IsSuccess.Should().BeTrue();
	}

	[Theory]
	[MemberData(nameof(InvalidPassword))]
	public void ReturnFailure_WhenPasswordIsInvalid(string password)
	{
		var result = _validator.Validate(password);
		result.IsFailure.Should().BeTrue();
	}

	public static IEnumerable<object[]> InvalidPassword()
	{
		yield return [string.Empty];
		yield return ["pP123aa"];
		yield return [string.Join(string.Empty, Enumerable.Range(1, 32 + 1).Select(_ => "a"))];
		yield return ["pppppp123"];
		yield return ["PPPPPP123"];
		yield return ["pppppPPPPP"];
	}
}
