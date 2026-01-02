using Articles.Application.Interfaces.Repositories;
using Articles.Application.Interfaces.Security;

namespace Articles.Application.AuthUseCases.Commands.ConfirmEmail;

internal sealed class ConfirmEmailCommandHandler(
	IEmailConfirmationTokenManager tokenManager,
	IUserRepository userRepository) : ICommandHandler<ConfirmEmailCommand>
{
	public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
	{
		var tokenResult = await tokenManager.DecryptToken(
			request.EmailConfirmationToken,
			cancellationToken);
		if (tokenResult.IsFailure)
		{
			return tokenResult.Error;
		}

		var emailToken = tokenResult.Value;
		var validationResult = tokenManager.Validate(emailToken);
		if (validationResult.IsFailure)
		{
			return validationResult.Error;
		}

		var userId = emailToken.UserId;
		var user = await userRepository.GetById(userId, cancellationToken);
		if (user is null)
		{
			return SecurityErrors.InvalidEmailConfirmationToken();
		}

		if (user.EmailConfirmed)
		{
			return SecurityErrors.EmailAlreadyConfirmed();
		}

		await userRepository.ConfirmEmail(userId, cancellationToken);
		return Result.Success();
	}
}
