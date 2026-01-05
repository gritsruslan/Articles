using Articles.API.Authentication;

namespace Articles.E2E;

public class LogoutTests(ArticlesWebApplicationFactory factory)
	: IClassFixture<ArticlesWebApplicationFactory>
{
	[Fact]
	public async Task SuccessfulLogoutAfterLogin()
	{
		using var client = factory.CreateClient();
		var email = "registerUse1r1@gmail.com";
		var password = "passwUser1";

		using var registerResponse = await client.PostAsync("auth/registration", JsonContent.Create(
			new RegistrationRequest("registerUser1", email, "regist1erUser1", password)));
		registerResponse.IsSuccessStatusCode.Should().BeTrue();

		using var loginResponse = await client.PostAsync("auth/login",
			JsonContent.Create(new LoginRequest(email, password, RememberMe : false)));
		loginResponse.IsSuccessStatusCode.Should().BeTrue();

		using var logoutResponse = await client.PostAsync("auth/logout", new StringContent(string.Empty));

		logoutResponse.IsSuccessStatusCode.Should().BeTrue();
		logoutResponse.HasMeaningfulCookie(AuthTokenHeaders.AccessToken).Should().BeFalse();
		logoutResponse.HasMeaningfulCookie(AuthTokenHeaders.RefreshToken).Should().BeFalse();
	}

	[Fact]
	public async Task FailureLogout_WhenEmptyAuthCredentials()
	{
		using var client = factory.CreateClient();

		using var response = await client.PostAsync("auth/logout", new StringContent(string.Empty));

		response.IsSuccessStatusCode.Should().BeFalse();
	}
}
