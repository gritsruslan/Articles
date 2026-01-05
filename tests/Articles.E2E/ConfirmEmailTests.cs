using Microsoft.Extensions.DependencyInjection;
using Articles.Application.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Application.Interfaces.Security;

namespace Articles.E2E;

public class ConfirmEmailTests(ArticlesWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
	: IClassFixture<ArticlesWebApplicationFactory>
{
	[Fact]
	private async Task SuccessfulConfirmEmail()
	{
		using var client = factory.CreateClient();
		var email = "testemailconfirm@gma.com";
		var password = "pAss123t";
		var userRepository = GetUserRepository();
		//singleton
		var tokenManager = factory.Services.GetRequiredService<IEmailConfirmationTokenManager>();

		using var registrationResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("refresh", email, "refresh2hh", password)));
		registrationResponse.IsSuccessStatusCode.Should().BeTrue();

		//get registered user and create email confirmation token for him
		var registeredUser = await userRepository
			.GetByEmail(Email.CreateVerified(email), CancellationToken.None);

		registeredUser.Should().NotBeNull();
		registeredUser.EmailConfirmed.Should().BeFalse();

		var token = await tokenManager.EncryptToken(new EmailConfirmationToken
		{
			UserId = registeredUser.Id,
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(1),
		}, CancellationToken.None);

		testOutputHelper.WriteLine($"Token : {token}");

		using var confirmEmailResponse = await client.PostAsync(
			$"auth/confirm-email?token={token}", new StringContent(string.Empty));
		confirmEmailResponse.IsSuccessStatusCode.Should().BeTrue();

		var user = await userRepository.GetByEmail(Email.CreateVerified(email), CancellationToken.None);
		user!.EmailConfirmed.Should().BeTrue();
	}

	[Fact]
	private async Task FailureConfirmEmail_WhenTokenIsInvalid()
	{
		using var client = factory.CreateClient();
		var email = "failedrefresh@gmail.com";

		using var registrationResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("refresh", email, "FAILEDREFRESH", "pssss123P")));
		registrationResponse.IsSuccessStatusCode.Should().BeTrue();

		var token = "INVALID_TOKEN";

		using var confirmEmailResponse = await client.PostAsync(
			$"auth/confirm-email?token={token}", new StringContent(string.Empty));
		confirmEmailResponse.IsSuccessStatusCode.Should().BeFalse();

		var user = await GetUserRepository().GetByEmail(Email.CreateVerified(email), CancellationToken.None);
		user!.EmailConfirmed.Should().BeFalse();
	}

	private IUserRepository GetUserRepository() =>
		factory.Services.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>();
}
