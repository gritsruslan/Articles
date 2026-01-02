using Articles.Application.AuthUseCases.Commands.RefreshTokens;
using Articles.Application.Interfaces.Repositories;
using Articles.Shared.DefaultServices;
using Articles.Shared.UnitOfWork;

namespace Articles.UnitTests.UseCases;

public class RefreshTokensCommandHandlerUnitTests
{
	private readonly RefreshTokensCommandHandler _refreshHandler;

	private readonly Mock<ISessionRepository> _sessionRepositoryMock;

	private readonly ISetup<ISessionRepository, Task<Session?>> _getSessionByIdSetup;

	private readonly ISetup<IRefreshTokenManager, Task<Result<RefreshToken>>> _decryptRefreshTokenSetup;

	private readonly ISetup<IRefreshTokenManager, Result> _validateRefreshTokenSetup;

	private readonly ISetup<ISessionManager, Result> _validateSessionSetup;

	private readonly ISetup<ISessionManager, Session> _createSessionSetup;

	private readonly RefreshTokensCommand _validCommand = new(string.Empty, string.Empty);

	public RefreshTokensCommandHandlerUnitTests()
	{
		_sessionRepositoryMock = new Mock<ISessionRepository>();
		_getSessionByIdSetup =
			_sessionRepositoryMock.Setup(r => r.GetById(It.IsAny<SessionId>(), It.IsAny<CancellationToken>()));
		_getSessionByIdSetup.Returns(Task.FromResult(new Session { UserAgent = "Windows 52" })!);

		var accessTokenManagerMock = new Mock<IAccessTokenManager>();

		var refreshTokenManagerMock = new Mock<IRefreshTokenManager>();
		_decryptRefreshTokenSetup =
			refreshTokenManagerMock.Setup(m => m.Decrypt(It.IsAny<string>(), It.IsAny<CancellationToken>()));
		_decryptRefreshTokenSetup.Returns(Task.FromResult(Result<RefreshToken>.Success(new RefreshToken
		{
			SessionId = SessionId.Parse("9FCE3556-A6DB-44BA-8890-583783CE0526"),
			UserId = UserId.Parse("90C2066C-B527-49B3-91FB-C35AA7A2D74F")
		})));

		_validateRefreshTokenSetup = refreshTokenManagerMock.Setup(m => m.Validate(It.IsAny<RefreshToken>()));
		_validateRefreshTokenSetup.Returns(Result.Success());

		var sessionManagerMock = new Mock<ISessionManager>();
		_validateSessionSetup = sessionManagerMock.Setup(m =>
			m.Validate(It.IsAny<Session>(), It.IsAny<UserId>(), It.IsAny<string>()));
		_validateSessionSetup.Returns(Result.Success());

		_createSessionSetup = sessionManagerMock
			.Setup(m => m.Create(It.IsAny<UserId>(), It.IsAny<string>(), It.IsAny<DateTime>()));
		_createSessionSetup.Returns(new Session
			{
				Id = SessionId.Parse("AFEFEE65-2F67-4149-875D-01DDC162E25D"),
				UserAgent = "Iphone 20"
			});

		var unitOfWorkMock = new Mock<IUnitOfWork>();
		unitOfWorkMock.Setup(u => u.StartScope(It.IsAny<CancellationToken>()))
			.Returns(Task.FromResult(new Mock<IUnitOfWorkScope>().Object));

		var dateTimeProviderMock = new Mock<IDateTimeProvider>();
		dateTimeProviderMock.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

		_refreshHandler = new RefreshTokensCommandHandler(
			_sessionRepositoryMock.Object,
			accessTokenManagerMock.Object,
			refreshTokenManagerMock.Object,
			sessionManagerMock.Object,
			unitOfWorkMock.Object,
			dateTimeProviderMock.Object);
	}

	[Fact]
	public async Task SuccessfullyRefreshTokens()
	{
		var oldSessionId = SessionId.Parse("4AD03F83-78AF-4608-8971-B0B65274C0A6");
		var newSessionId = SessionId.Parse("A42F006E-B859-4373-AF08-A8B83697CEE9");
		var userId = UserId.Parse("C682E008-98B3-4D76-B454-1BBDACA6A475");

		_decryptRefreshTokenSetup.Returns(Task.FromResult(Result<RefreshToken>.Success(
			new RefreshToken { SessionId = oldSessionId, UserId = userId })));
		_getSessionByIdSetup.Returns(Task.FromResult(new Session
		{
			Id = oldSessionId,
			UserId = userId,
			UserAgent = "Windows 52"
		})!);
		_createSessionSetup.Returns(new Session
		{
			Id = newSessionId,
			UserId = userId,
			UserAgent = "Windows 52"
		});

		var result = await _refreshHandler.Handle(_validCommand, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		_sessionRepositoryMock.Verify(r => r.DeleteById(oldSessionId, It.IsAny<CancellationToken>()), Times.Once);
		_sessionRepositoryMock.Verify(r => r.Add(
			It.Is<Session>(s =>
				s.Id == newSessionId &&
				s.UserId == userId),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task FailureRefresh_WhenFailedToDecryptRefreshToken()
	{
		_decryptRefreshTokenSetup.Returns(Task.FromResult(Result<RefreshToken>.EmptyFailure()));

		var result = await _refreshHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
	}

	[Fact]
	public async Task FailureRefresh_WhenRefreshTokenIsInvalid()
	{
		_validateRefreshTokenSetup.Returns(Result.EmptyFailure());

		var result = await _refreshHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
	}

	[Fact]
	public async Task FailureRefresh_WhenSessionDoesNotExist()
	{
		_getSessionByIdSetup.Returns(Task.FromResult<Session?>(null));

		var result = await _refreshHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
	}

	[Fact]
	public async Task FailureRefresh_WhenSessionAndRefreshTokenDataNotMatch()
	{
		_validateSessionSetup.Returns(Result.EmptyFailure());

		var result = await _refreshHandler.Handle(_validCommand, CancellationToken.None);

		result.IsFailure.Should().BeTrue();
	}
}
