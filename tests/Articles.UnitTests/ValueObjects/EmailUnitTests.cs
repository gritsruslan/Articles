namespace Articles.UnitTests.ValueObjects;

public class EmailUnitTests
{
	[Theory]
	[InlineData("dima228@gmail.com")]
	[InlineData("h1@sho.ua")]
	[InlineData("email@hello.ba")]
	public void CreateEmail_WhenItIsValid(string emailStr)
	{
		var result = Email.Create(emailStr);
		result.IsSuccess.Should().BeTrue();
	}

	[Theory]
	[InlineData("user@@example.com")]
	[InlineData("userexample.com")]
	[InlineData("user@.com")]
	[InlineData("@example.com")]
	[InlineData("user@exam_ple..com")]
	public void DontCreateEmail_WhenItIsNotValid(string emailStr)
	{
		var result = Email.Create(emailStr);
		result.IsFailure.Should().BeTrue();
	}
}
