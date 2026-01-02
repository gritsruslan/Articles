using Articles.Application.Authentication;
using Articles.Application.Interfaces.Authentication;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Identifiers;

namespace Articles.Infrastructure.Authentication;

internal sealed class AuthenticationService(
	IRoleManager roleManager,
	IUserRepository userRepository) : IAuthenticationService
{
	public async Task<RecognizedUser> Authenticate(UserId userId, CancellationToken cancellationToken)
	{
		var user = await userRepository.GetById(userId, cancellationToken);
		if (user is null)
		{
			return new RecognizedUser
			{
				Id = UserId.Empty,
				Role = roleManager.Guest()
			};
		}

		var role = roleManager.GetRole(user.RoleId);
		return new RecognizedUser
		{
			Id = userId,
			Email = user.Email,
			EmailConfirmed = user.EmailConfirmed,
			Role = role
		};
	}
}
