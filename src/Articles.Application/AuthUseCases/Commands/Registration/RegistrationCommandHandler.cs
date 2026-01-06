using Articles.Application.Interfaces.Repositories;
using Articles.Application.Interfaces.Security;
using Articles.Domain.DomainEvents;
using Articles.Domain.Enums;
using Articles.Shared.UnitOfWork;

namespace Articles.Application.AuthUseCases.Commands.Registration;

internal sealed class RegistrationCommandHandler(
	IPasswordHasher passwordHasher,
	IPasswordValidator passwordValidator,
	IUserRepository repository,
	IDomainEventRepository domainEventRepository,
	IUnitOfWork unitOfWork) : ICommandHandler<RegistrationCommand>
{
	public async Task<Result> Handle(RegistrationCommand request, CancellationToken cancellationToken)
	{
		var emailResult = Email.Create(request.Email);
		if (emailResult.IsFailure)
		{
			return emailResult.Error;
		}

		var domainIdResult = DomainId.Create(request.DomainId);
		if (domainIdResult.IsFailure)
		{
			return domainIdResult.Error;
		}

		var userNameResult = UserName.Create(request.UserName);
		if (userNameResult.IsFailure)
		{
			return userNameResult.Error;
		}
		var name = userNameResult.Value;

		var passwordValidationResult = passwordValidator.Validate(request.Password);
		if (passwordValidationResult.IsFailure)
		{
			return passwordValidationResult.Error;
		}

		var email = emailResult.Value;
		var domainId = domainIdResult.Value;
		var password = request.Password;

		if (await repository.ExistsByEmail(email, cancellationToken))
		{
			return UserErrors.UserWithThisEmailAlreadyExists(email.Value);
		}

		if (await repository.ExistsByDomainId(domainId, cancellationToken))
		{
			return UserErrors.UserWithThisDomainIdAlreadyExists(domainId.Value);
		}

		var salt = passwordHasher.GenerateSalt();
		var passwordHash = passwordHasher.HashPassword(password, salt);

		var user = new User
		{
			Id = UserId.New(),
			Name = name,
			Email = email,
			RoleId = RoleId.Create((int)Roles.User),
			DomainId = domainId,
			EmailConfirmed = false,
			PasswordHash = passwordHash,
			Salt = salt
		};

		await using var scope = await unitOfWork.StartScope(cancellationToken);

		await repository.Add(user, cancellationToken);
		await domainEventRepository.Add(
			new UserRegisteredDomainEvent(email.Value, name.Value), cancellationToken);

		await scope.Commit(cancellationToken);

		return Result.Success();
	}
}
