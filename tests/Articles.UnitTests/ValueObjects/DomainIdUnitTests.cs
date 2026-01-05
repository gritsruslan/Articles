namespace Articles.UnitTests.ValueObjects;

public class DomainIdUnitTests
{
	[Theory]
	[InlineData("hehe228")]
	[InlineData("_TO_")]
	[InlineData("user")]
	[InlineData("lexa_krutoy228")]
	public void CreateDomainId_WhenItIsValid(string domainIdStr)
	{
		var result = DomainId.Create(domainIdStr);
		result.IsSuccess.Should().BeTrue();
	}

	[Theory]
	[InlineData("")]
	[InlineData("		")]
	[InlineData("hah")]
	[InlineData("1hahhah")]
	[InlineData("HahHahHahHahHah0")]
	public void DontCreateDomainId_WhenItIsInvalid(string domainIdStr)
	{
		var result = DomainId.Create(domainIdStr);
		result.IsSuccess.Should().BeFalse();
	}
}
