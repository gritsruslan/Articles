using Articles.API.Authentication;

namespace Articles.E2E;

public class LoginTests(ArticlesWebApplicationFactory factory)
	: IClassFixture<ArticlesWebApplicationFactory>
{
	[Fact]
	public async Task SuccessfulLoginAfterRegistration()
	{
		using var client = factory.CreateClient();
		var email = "myemail@gmail.com";
		var password = "MYPASSs52";

		using var registerResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("tName", email, "tDom2ainId", password)));
		registerResponse.IsSuccessStatusCode.Should().BeTrue();

		using var loginResponse = await client.PostAsync("auth/login",
			JsonContent.Create(new LoginRequest(email, password, RememberMe: false)));

		loginResponse.IsSuccessStatusCode.Should().BeTrue();
		loginResponse.HasMeaningfulCookie(AuthTokenHeaders.AccessToken).Should().BeTrue();
		loginResponse.HasMeaningfulCookie(AuthTokenHeaders.RefreshToken).Should().BeTrue();
	}

	[Fact]
	public async Task FailureLogin_IfUserDoesNotExist()
	{
		using var client = factory.CreateClient();

		using var response = await client.PostAsync("auth/login",
			JsonContent.Create(new LoginRequest("invalidEma1l@ukr.net", "somepassww123", RememberMe: false)));

		response.IsSuccessStatusCode.Should().BeFalse();
		response.HasMeaningfulCookie(AuthTokenHeaders.AccessToken).Should().BeFalse();
		response.HasMeaningfulCookie(AuthTokenHeaders.RefreshToken).Should().BeFalse();
	}

	[Fact]
	public async Task FailureLogin_WhenPasswordsNotMatch()
	{
		using var client = factory.CreateClient();
		var email = "secondTest@gmail.com";
		var password = "MYPASSs52";
		var invalidPass = "525252525haha";

		using var registerResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("name", email, "anyDomainId1", password)));
		registerResponse.IsSuccessStatusCode.Should().BeTrue();

		using var loginResponse = await client.PostAsync("auth/login",
			JsonContent.Create(new LoginRequest(email, invalidPass, RememberMe: false)));

		loginResponse.IsSuccessStatusCode.Should().BeFalse();
		loginResponse.HasMeaningfulCookie(AuthTokenHeaders.AccessToken).Should().BeFalse();
		loginResponse.HasMeaningfulCookie(AuthTokenHeaders.RefreshToken).Should().BeFalse();
	}

}
