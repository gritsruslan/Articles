using Articles.Application.AuthUseCases.Commands.ConfirmEmail;
using Articles.Application.Interfaces.Repositories;

namespace Articles.UnitTests.UseCases;

public class ConfirmEmailCommandHandlerUnitTests
{
	private readonly ConfirmEmailCommandHandler _confirmEmailCommandHandler;

	private readonly Mock<IUserRepository> _userRepositoryMock;

	private readonly ISetup<IUserRepository, Task<User?>> _getUserByIdSetup;

	private readonly ISetup<IEmailConfirmationTokenManager, Task<Result<EmailConfirmationToken>>> _decryptTokenSetup;

	private readonly ISetup<IEmailConfirmationTokenManager, Result> _validateTokenSetup;

	private readonly ConfirmEmailCommand _validCommand = new(string.Empty);

	public ConfirmEmailCommandHandlerUnitTests()
	{
		var tokenManagerMock = new Mock<IEmailConfirmationTokenManager>();
		_decryptTokenSetup =
			tokenManagerMock.Setup(m => m.DecryptToken(It.IsAny<string>(), It.IsAny<CancellationToken>()));
		_decryptTokenSetup.Returns(Task.FromResult(Result<EmailConfirmationToken>.Success(
			new EmailConfirmationToken())));

		_validateTokenSetup = tokenManagerMock.Setup(m => m.Validate(It.IsAny<EmailConfirmationToken>()));
		_validateTokenSetup.Returns(Result.Success());

		_userRepositoryMock = new Mock<IUserRepository>();
		_getUserByIdSetup =
			_userRepositoryMock.Setup(r => r.GetById(It.IsAny<UserId>(), It.IsAny<CancellationToken>()));
		_getUserByIdSetup.Returns(Task.FromResult(new User { PasswordHash = [], Salt = [], })!);

		_confirmEmailCommandHandler =
			new ConfirmEmailCommandHandler(tokenManagerMock.Object, _userRepositoryMock.Object);
	}

	[Fact]
	public async Task SuccessfulConfirmEmail()
	{
		var userId = UserId.Parse("63919753-AC45-44FA-9002-1B134CE13C1B");
		_decryptTokenSetup.Returns(Task.FromResult(Result<EmailConfirmationToken>.Success(
			new EmailConfirmationToken()
			{
				UserId = userId
			})));

		var result = await _confirmEmailCommandHandler.Handle(_validCommand, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		_userRepositoryMock.Verify(r => r.ConfirmEmail(userId, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task NotConfirm_WhenFailedToDecryptConfirmationToken()
	{
		_decryptTokenSetup.Returns(Task.FromResult(Result<EmailConfirmationToken>.EmptyFailure()));

		var result = await _confirmEmailCommandHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_userRepositoryMock.Verify(r => r.ConfirmEmail(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task NotConfirm_WhenConfirmationTokenIsInvalid()
	{
		_validateTokenSetup.Returns(Result.EmptyFailure());

		var result = await _confirmEmailCommandHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_userRepositoryMock.Verify(r => r.ConfirmEmail(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task NotConfirm_WhenUserNotFound()
	{
		_getUserByIdSetup.Returns(Task.FromResult<User?>(null));

		var result = await _confirmEmailCommandHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_userRepositoryMock.Verify(r => r.ConfirmEmail(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task ReturnFailure_WhenEmailAlreadyConfirmed()
	{
		_getUserByIdSetup.Returns(Task.FromResult(new User
		{
			PasswordHash = [],
			Salt = [],
			EmailConfirmed = true,
		})!);

		var result = await _confirmEmailCommandHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_userRepositoryMock.Verify(r => r.ConfirmEmail(It.IsAny<UserId>(), It.IsAny<CancellationToken>()), Times.Never);
	}
}
