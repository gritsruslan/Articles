using Articles.Application.AuthUseCases.Commands.Registration;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Errors;

namespace Articles.UnitTests.UseCases;

public class RegistrationCommandHandlerUnitTests
{
	private readonly RegistrationCommandHandler _registrationHandler;

	private readonly Mock<IUserRepository> _userRepositoryMock;

	private readonly ISetup<IUserRepository, Task<bool>> _userExistsByEmailSetup;

	private readonly ISetup<IUserRepository, Task<bool>> _userExistsByDomainIdSetup;

	private readonly ISetup<IPasswordValidator, Result> _passwordValidatorSetup;

	private readonly RegistrationCommand _validCommand = new(
		"MyUserName",
		"myemail@google.com",
		"MyDomainId",
		"MyStrongPass123");

	public RegistrationCommandHandlerUnitTests()
	{
		var passwordHasherMock = new Mock<IPasswordHasher>();

		var passwordValidatorMock = new Mock<IPasswordValidator>();
		_passwordValidatorSetup = passwordValidatorMock.Setup(p => p.Validate(It.IsAny<string>()));
		_passwordValidatorSetup.Returns(Result.Success());

		_userRepositoryMock = new Mock<IUserRepository>();
		_userExistsByEmailSetup = _userRepositoryMock
			.Setup(r => r.ExistsByEmail(It.IsAny<Email>(), It.IsAny<CancellationToken>()));
		_userExistsByEmailSetup.Returns(Task.FromResult(false));

		_userExistsByDomainIdSetup = _userRepositoryMock
			.Setup(r => r.ExistsByDomainId(It.IsAny<DomainId>(), It.IsAny<CancellationToken>()));
		_userExistsByDomainIdSetup.Returns(Task.FromResult(false));

		_registrationHandler = new RegistrationCommandHandler(
			passwordHasherMock.Object,
			passwordValidatorMock.Object,
			_userRepositoryMock.Object);
	}

	[Fact]
	public async Task SuccessfulRegisterUser()
	{
		var result = await _registrationHandler.Handle(_validCommand, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		_userRepositoryMock.Verify(
			r => r.Add(
				It.Is<User>(
					u => u.Name.Value == _validCommand.UserName &&
					     u.Email.Value == _validCommand.Email &&
					     u.DomainId.Value == _validCommand.DomainId),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task NotRegister_WhenEmailIsInvalid()
	{
		var invalidCommand = _validCommand with { Email = "invalid@@gmail.ua" };

		var result = await _registrationHandler.Handle(invalidCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.InvalidValue);
		_userRepositoryMock.Verify(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task NotRegisterUser_WhenDomainIdIsInvalid()
	{
		var invalidCommand = _validCommand with { DomainId = "inv" };

		var result = await _registrationHandler.Handle(invalidCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.InvalidValue);
		_userRepositoryMock.Verify(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task NotRegisterUser_WhenPasswordIsInvalid()
	{
		_passwordValidatorSetup.Returns(new Result(new Error(ErrorType.InvalidValue)));

		var result = await _registrationHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.InvalidValue);
		_userRepositoryMock.Verify(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task NotRegisterUser_WhenUserWithSuchEmailAlreadyExists()
	{
		_userExistsByEmailSetup.Returns(Task.FromResult(true));

		var result = await _registrationHandler.Handle(_validCommand,CancellationToken.None);

		result.IsSuccess.Should().BeFalse();
		result.Error.Should().BeEquivalentTo(
			UserErrors.UserWithThisEmailAlreadyExists(_validCommand.Email));
		_userRepositoryMock.Verify(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task NotRegisterUser_WhenUserWithSuchDomainIdAlreadyExists()
	{
		_userExistsByDomainIdSetup.Returns(Task.FromResult(true));

		var result = await _registrationHandler.Handle(_validCommand,CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Should().BeEquivalentTo(
			UserErrors.UserWithThisDomainIdAlreadyExists(_validCommand.DomainId));
		_userRepositoryMock.Verify(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}
}
