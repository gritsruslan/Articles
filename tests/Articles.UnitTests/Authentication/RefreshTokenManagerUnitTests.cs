using Articles.Infrastructure.Authentication;
using Articles.Infrastructure.Security;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Articles.UnitTests.Authentication;

public class RefreshTokenManagerUnitTests
{
	private readonly IRefreshTokenManager _refreshTokenManager;

	private readonly Mock<ISymmetricCryptoService> _cryptoServiceMock;

	private readonly Mock<IOptions<RefreshTokenOptions>> _optionsMock;

	private readonly ISetup<IOptions<RefreshTokenOptions>, RefreshTokenOptions> _optionsSetup;

	private readonly ISetup<ISymmetricCryptoService, Task<string>> _decryptionSetup;

	public RefreshTokenManagerUnitTests()
	{
		_optionsMock = new Mock<IOptions<RefreshTokenOptions>>();
		_optionsSetup = _optionsMock.Setup(x => x.Value);
		_optionsSetup.Returns(new RefreshTokenOptions()
		{
			Key = "1111111111111111",
			Issuer = "issuer",
			Audience = "audience"
		});

		var realCryptoService = new AesSymmetricCryptoService();
		_cryptoServiceMock = new Mock<ISymmetricCryptoService>();
		_cryptoServiceMock.Setup(s => s.EncryptAsync(
			It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
			.Returns((string plainText, byte[] key, CancellationToken ct) =>
				realCryptoService.EncryptAsync(plainText, key, ct));
		_decryptionSetup = _cryptoServiceMock.Setup(s =>
			s.DecryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));
		_decryptionSetup.Returns((string plainText, byte[] key, CancellationToken ct) =>
			realCryptoService.DecryptAsync(plainText, key, ct));

		_refreshTokenManager = new RefreshTokenManager(
			_optionsMock.Object,
			NullLogger<RefreshTokenManager>.Instance,
			_cryptoServiceMock.Object);
	}

	[Fact]
	public async Task CreateMeaningfulEncryptedToken()
	{
		var userId = UserId.Parse("945B690D-8F18-49F9-AB4E-FF3D4A807E53");
		var sessionId = SessionId.Parse("945B690D-8F18-49F9-AB4E-FF3D4A807E53");

		var token = await _refreshTokenManager.CreateEncrypted(userId, sessionId, CancellationToken.None);
		token.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public void ReturnSuccessfulValidation()
	{
		var token = new RefreshToken
		{
			Issuer = "issuer",
			Audience = "audience"
		};

		var validationResult = _refreshTokenManager.Validate(token);
		validationResult.IsSuccess.Should().BeTrue();
	}

	[Fact]
	public void ReturnFailureValidation_WhenIssuerIsInvalid()
	{
		var token = new RefreshToken
		{
			Issuer = "INVALID",
			Audience = "audience"
		};

		var validationResult = _refreshTokenManager.Validate(token);
		validationResult.IsSuccess.Should().BeFalse();
		validationResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public void ReturnFailureValidation_WhenAudienceIsInvalid()
	{
		var token = new RefreshToken
		{
			Issuer = "issuer",
			Audience = "INVALID"
		};

		var validationResult = _refreshTokenManager.Validate(token);
		validationResult.IsSuccess.Should().BeFalse();
		validationResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public async Task SuccessfullyDecryptToken()
	{
		var userId = UserId.Parse("E66B5BD2-FA96-4825-A226-BD357C7F7155");
		var sessionId = SessionId.Parse("E66B5BD2-FA96-4825-A226-BD357C7F7155");

		var token = await _refreshTokenManager.CreateEncrypted(userId, sessionId, CancellationToken.None);

		var decryptResult = await _refreshTokenManager.Decrypt(token, CancellationToken.None);
		decryptResult.IsSuccess.Should().BeTrue();
		decryptResult.Value.UserId.Should().Be(userId);
		decryptResult.Value.SessionId.Should().Be(sessionId);
	}

	[Fact]
	public async Task ReturnFailureDecryption_WhenInvalidKey()
	{
		var userId = UserId.Parse("BD5B2E65-4789-4386-9DD1-00D5A0855FC9");
		var sessionId = SessionId.Parse("BD5B2E65-4789-4386-9DD1-00D5A0855FC9");
		var token = await _refreshTokenManager.CreateEncrypted(userId, sessionId, CancellationToken.None);

		_optionsSetup.Returns(new RefreshTokenOptions()
		{
			Key = "invalid_key11111",
			Issuer = "issuer",
			Audience = "audience"
		});
		var newTokenManager = new RefreshTokenManager(
			_optionsMock.Object,
			NullLogger<RefreshTokenManager>.Instance,
			_cryptoServiceMock.Object);

		var decryptionResult = await newTokenManager.Decrypt(token, CancellationToken.None);
		decryptionResult.IsFailure.Should().BeTrue();
		decryptionResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public async Task ReturnFailureDecryption_WhenCannotConvertJsonToToken()
	{
		_decryptionSetup.Returns(Task.FromResult("{}")); //empty json

		var decryptionResult = await _refreshTokenManager.Decrypt(string.Empty, CancellationToken.None);
		decryptionResult.IsFailure.Should().BeTrue();
		decryptionResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}
}
