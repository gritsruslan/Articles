using Articles.Application.Interfaces.Repositories;
using Articles.Application.Interfaces.Security;
using Articles.Domain.DomainEvents;
using Articles.Domain.Enums;
using Articles.Shared.Abstraction.CQRS;
using Articles.Shared.UnitOfWork;

namespace Articles.Application.UseCases.Auth.Registration;

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

		await using var scope = await unitOfWork.StartScope(cancellationToken);

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

		var id = UserId.New();
		var user = new User
		{
			Id = id,
			Name = name,
			Email = email,
			RoleId = RoleId.Create((int)Roles.User),
			DomainId = domainId,
			EmailConfirmed = false,
			PasswordHash = passwordHash,
			Salt = salt
		};

		await repository.Add(user, cancellationToken);
		await domainEventRepository.Add(
			new UserRegisteredDomainEvent(id.Value, email.Value, name.Value), cancellationToken);

		await scope.Commit(cancellationToken);

		return Result.Success();
	}
}
