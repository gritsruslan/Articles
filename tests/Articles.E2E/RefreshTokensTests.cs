using Articles.API.Authentication;

namespace Articles.E2E;

public class RefreshTokensTests(ArticlesWebApplicationFactory factory)
	: IClassFixture<ArticlesWebApplicationFactory>
{
	[Fact]
	private async Task SuccessfullyRefreshTokensAfterLogin()
	{
		using var client = factory.CreateClient();
		var email = "refreshTest@gmail.com";
		var password = "rEfReShhh1337";

		using var registrationResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("refresh", email, "refreshhh", password)));
		registrationResponse.IsSuccessStatusCode.Should().BeTrue();

		using var loginResponse = await client.PostAsync("auth/login",
			JsonContent.Create(new LoginRequest(email, password, RememberMe : true)));
		loginResponse.IsSuccessStatusCode.Should().BeTrue();

		await Task.Delay(100); // wait a bit

		using var refreshResponse = await client.PostAsync("auth/refresh", new StringContent(string.Empty));
		refreshResponse.IsSuccessStatusCode.Should().BeTrue();

		var oldAccess = loginResponse.GetCookie(AuthTokenHeaders.AccessToken);
		var oldRefresh = loginResponse.GetCookie(AuthTokenHeaders.RefreshToken);
		var newAccess = refreshResponse.GetCookie(AuthTokenHeaders.AccessToken);
		var newRefresh = refreshResponse.GetCookie(AuthTokenHeaders.RefreshToken);

		newAccess.Should().NotBeNullOrWhiteSpace().And.NotBe(oldAccess);
		newRefresh.Should().NotBeNullOrWhiteSpace().And.NotBe(oldRefresh);
	}

	[Fact]
	private async Task FailureRefreshTokens_WhenNoUserCredentials()
	{
		using var client = factory.CreateClient();

		using var response = await client.PostAsync("auth/refresh", new StringContent(string.Empty));

		response.IsSuccessStatusCode.Should().BeFalse();
		response.HasMeaningfulCookie(AuthTokenHeaders.AccessToken).Should().BeFalse();
		response.HasMeaningfulCookie(AuthTokenHeaders.RefreshToken).Should().BeFalse();
	}

}
