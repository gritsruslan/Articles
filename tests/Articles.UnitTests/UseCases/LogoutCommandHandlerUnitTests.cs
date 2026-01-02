using Articles.Application.AuthUseCases.Commands.Logout;
using Articles.Application.Interfaces.Repositories;

namespace Articles.UnitTests.UseCases;

public class LogoutCommandHandlerUnitTests
{
	private readonly LogoutCommandHandler _logoutHandler;

	private readonly ISetup<IRefreshTokenManager, Task<Result<RefreshToken>>> _decryptRefreshTokenSetup;

	private readonly ISetup<IRefreshTokenManager, Result> _validateRefreshTokenSetup;

	private readonly Mock<ISessionRepository> _sessionRepositoryMock;

	private readonly ISetup<ISessionRepository, Task<Session?>> _getSessionSetup;

	private readonly LogoutCommand _validCommand = new(string.Empty);

	public LogoutCommandHandlerUnitTests()
	{
		var refreshTokenManagerMock = new Mock<IRefreshTokenManager>();

		_decryptRefreshTokenSetup = refreshTokenManagerMock
			.Setup(r => r.Decrypt(It.IsAny<string>(), It.IsAny<CancellationToken>()));
		_decryptRefreshTokenSetup.Returns(Task.FromResult(Result<RefreshToken>.Success(new RefreshToken())));

		_validateRefreshTokenSetup = refreshTokenManagerMock
			.Setup(r => r.Validate(It.IsAny<RefreshToken>()));
		_validateRefreshTokenSetup.Returns(Result.Success());

		var applicationUserProviderMock = new Mock<IApplicationUserProvider>();
		applicationUserProviderMock.Setup(p => p.CurrentUser).Returns(
			new RecognizedUser
			{
				Role = new Role { Name = "user", Permissions = [] }
			});

		_sessionRepositoryMock = new Mock<ISessionRepository>();
		_getSessionSetup = _sessionRepositoryMock.Setup(r =>
			r.GetById(It.IsAny<SessionId>(), It.IsAny<CancellationToken>()));
		_getSessionSetup.Returns(Task.FromResult<Session?>(new Session
		{
			UserAgent = "Windows 52"
		}));

		_logoutHandler = new LogoutCommandHandler(
			refreshTokenManagerMock.Object,
			applicationUserProviderMock.Object,
			_sessionRepositoryMock.Object);
	}

	[Fact]
	public async Task SuccessfullyLogout()
	{
		var sessionId = SessionId.Parse("10A11F29-034E-4AD0-886A-691439AAD5BB");
		_decryptRefreshTokenSetup.Returns(Task.FromResult(Result<RefreshToken>.Success(
			new RefreshToken {SessionId = sessionId})));

		var result = await _logoutHandler.Handle(_validCommand, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		_sessionRepositoryMock.Verify(s =>
			s.DeleteById(sessionId, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task FailureLogout_WhenFailedToDecryptRefreshToken()
	{
		_decryptRefreshTokenSetup.Returns(
			Task.FromResult(Result<RefreshToken>.EmptyFailure()));

		var result = await _logoutHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_sessionRepositoryMock.Verify(s => s.DeleteById(It.IsAny<SessionId>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task FailureLogout_WhenRefreshTokenIsInvalid()
	{
		_validateRefreshTokenSetup.Returns(Result.EmptyFailure());

		var result = await _logoutHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_sessionRepositoryMock.Verify(s => s.DeleteById(It.IsAny<SessionId>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task FailureLogout_WhenSessionDoesNotExist()
	{
		_getSessionSetup.Returns(Task.FromResult<Session?>(null));

		var result = await _logoutHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
		_sessionRepositoryMock.Verify(s => s.DeleteById(It.IsAny<SessionId>(), It.IsAny<CancellationToken>()), Times.Never);
	}
}
