using Microsoft.Extensions.DependencyInjection;
using Articles.Application.AuthUseCases.Commands.Registration;
using Articles.Application.Interfaces.Repositories;

namespace Articles.E2E;

public class RegistrationTest(ArticlesWebApplicationFactory factory)
	: IClassFixture<ArticlesWebApplicationFactory>
{
	private IUserRepository GetUserRepository() =>
		factory.Services.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>();

	[Fact]
	public async Task SuccessfulRegistration()
	{
		using var client = factory.CreateClient();
		var email = "tt2est1523@gi.com";
		var userRepository = GetUserRepository();

		using var response = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("tName", email, "asdfasdf", "pAss1235")));

		response.IsSuccessStatusCode.Should().BeTrue();
		var userExists = await userRepository.ExistsByEmail(Email.CreateVerified(email), CancellationToken.None);
		userExists.Should().BeTrue();
	}

	[Fact]
	public async Task FailureRegistration_WhenUserWithEmailAlreadyExists()
	{
		using var client = factory.CreateClient();
		var email = "notunique@gmail.com";

		using var firstResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("tName", email, "tDomainId33", "pAss1235")));
		firstResponse.IsSuccessStatusCode.Should().BeTrue();

		using var secondResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationCommand("tName", email, "tDomainId53", "pAss1235")));
		secondResponse.IsSuccessStatusCode.Should().BeFalse();
	}

	[Fact]
	public async Task FailureRegistration_WhenUserWithDomainIdAlreadyExists()
	{
		using var client = factory.CreateClient();
		var domainId = "notunique52";

		using var firstResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationRequest("tName", "any52@sss.com", domainId, "pAss1235")));
		firstResponse.IsSuccessStatusCode.Should().BeTrue();

		using var secondResponse = await client.PostAsync("auth/registration",
			JsonContent.Create(new RegistrationCommand("tName", "ihateni@hello.uk", domainId, "pAss1235")));
		secondResponse.IsSuccessStatusCode.Should().BeFalse();
	}

}
