using Articles.Application.AuthUseCases.Commands.Login;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.Errors;

namespace Articles.UnitTests.UseCases;

public class LoginCommandHandlerUnitTests
{
	private readonly LoginCommandHandler _loginHandler;

	private readonly Mock<ISessionRepository> _sessionRepositoryMock;

	private readonly ISetup<IUserRepository, Task<User?>> _getUserByEmailSetup;

	private readonly ISetup<IPasswordHasher, bool> _verifyPasswordSetup;

	private readonly ISetup<ISessionManager, Session> _createSessionSetup;

	private readonly LoginCommand _validCommand = new("user@gmail.com", "MyPass1234", true, "Windows 10");

	public LoginCommandHandlerUnitTests()
	{
		var userRepositoryMock = new Mock<IUserRepository>();
		_getUserByEmailSetup =
			userRepositoryMock.Setup(r => r.GetByEmail(It.IsAny<Email>(), It.IsAny<CancellationToken>()));
		_getUserByEmailSetup.Returns(Task.FromResult(new User
		{
			Id = UserId.Parse("E6BEAF48-D968-41DD-8CA3-2BD1F460727F"),
			DomainId = DomainId.CreateVerified("user"),
			Email = Email.CreateVerified("user@gmail.com"),
			PasswordHash = [],
			Salt = []
		})!);

		_sessionRepositoryMock = new Mock<ISessionRepository>();

		var passwordHasherMock = new Mock<IPasswordHasher>();
		_verifyPasswordSetup = passwordHasherMock.Setup(
			p => p.VerifyPassword(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<string>()));
		_verifyPasswordSetup.Returns(true);

		var accessTokenManagerMock = new Mock<IAccessTokenManager>();
		var refreshTokenManagerMock = new Mock<IRefreshTokenManager>();

		var sessionManager = new Mock<ISessionManager>();
		_createSessionSetup = sessionManager.Setup(s =>
			s.Create(It.IsAny<UserId>(), It.IsAny<string>(), It.IsAny<bool>()));
		_createSessionSetup.Returns(new Session { UserAgent = "Windows 10" });

		_loginHandler = new LoginCommandHandler(
			userRepositoryMock.Object,
			_sessionRepositoryMock.Object,
			passwordHasherMock.Object,
			accessTokenManagerMock.Object,
			refreshTokenManagerMock.Object,
			sessionManager.Object);
	}

	[Fact]
	public async Task SuccessfullyLogin_ReturningAccessAndRefreshTokens()
	{
		var userId = UserId.Parse("E6BEAF48-D968-41DD-8CA3-2BD1F460727F");
		_getUserByEmailSetup.Returns(Task.FromResult(new User
		{
			Id = userId,
			DomainId = DomainId.CreateVerified("user"),
			Email = Email.CreateVerified("user@gmail.com"),
			PasswordHash = [],
			Salt = []
		})!);
		_createSessionSetup.Returns(new Session {UserId = userId, UserAgent = "Windows 10" });

		var result = await _loginHandler.Handle(_validCommand, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		result.Value.Should().BeOfType<AuthTokenPair>().And.NotBeNull();
		_sessionRepositoryMock.Verify(r => r.Add(
			It.Is<Session>(s => s.UserId == userId),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task FailureLogin_WhenEmailIsInvalid()
	{
		var invalidCommand = _validCommand with { Email = "invalid.gmail.com" };

		var result = await _loginHandler.Handle(invalidCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Should().BeEquivalentTo(SecurityErrors.IncorrectEmailOrPassword());
		_sessionRepositoryMock.Verify(r => r.Add(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task FailureLogin_WhenUserDoesNotExistByEmail()
	{
		_getUserByEmailSetup.Returns(Task.FromResult<User?>(null));

		var result = await _loginHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Should().BeEquivalentTo(SecurityErrors.IncorrectEmailOrPassword());
		_sessionRepositoryMock.Verify(r => r.Add(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task FailureLogin_WhenIncorrectPassword()
	{
		_verifyPasswordSetup.Returns(false);

		var result = await _loginHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		result.Error.Should().BeEquivalentTo(SecurityErrors.IncorrectEmailOrPassword());
		_sessionRepositoryMock.Verify(r => r.Add(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Never);
	}
}
