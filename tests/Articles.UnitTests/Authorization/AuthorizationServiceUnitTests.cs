using Articles.Application.Authorization;
using Articles.Domain.Enums;
using Articles.Infrastructure.Authorization;

namespace Articles.UnitTests.Authorization;

public class AuthorizationServiceUnitTests
{
	private readonly IAuthorizationService _authorizationService;

	private readonly ISetup<IApplicationUserProvider, RecognizedUser> _userProviderSetup;

	public AuthorizationServiceUnitTests()
	{
		var userProviderMock = new Mock<IApplicationUserProvider>();
		_userProviderSetup = userProviderMock.Setup(up => up.CurrentUser);

		_authorizationService = new AuthorizationService(userProviderMock.Object);
	}

	[Fact]
	public void ReturnSuccessResult()
	{
		_userProviderSetup.Returns(new RecognizedUser
		{
			Role = new Role
			{
				Id = RoleId.Create((int)Roles.User),
				Name = "",
				Permissions = [new Permission
				{
					Id = 1,
					Name = "Correct"
				}]
			}
		});

		var result = _authorizationService.IsAllowed(1);

		result.IsSuccess.Should().BeTrue();
	}

	[Fact]
	public void ReturnUnauthorizedError_WhenUserIsGuest()
	{
		_userProviderSetup.Returns(new RecognizedUser
		{
			Role = new Role
			{
				Id = RoleId.Create((int)Roles.Guest),
				Name = "",
				Permissions = []
			}
		});

		var result = _authorizationService.IsAllowed(1);

		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public void ReturnForbiddenError_WhenUserDontHavePermission()
	{
		_userProviderSetup.Returns(new RecognizedUser
		{
			Role = new Role
			{
				Id = RoleId.Create((int)Roles.User),
				Name = "",
				Permissions = [ new Permission
				{
					Id = 1,
					Name = "Invalid"
				}]
			}
		});

		var result = _authorizationService.IsAllowed(2);

		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.Forbidden);
	}
}
