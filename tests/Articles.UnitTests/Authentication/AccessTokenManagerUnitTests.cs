using Articles.Infrastructure.Authentication;
using Articles.Infrastructure.Security;
using Articles.Shared.DefaultServices;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Articles.UnitTests.Authentication;

public class AccessTokenManagerUnitTests
{
	private readonly IAccessTokenManager _accessTokenManager;

	private readonly Mock<IOptions<AccessTokenOptions>> _optionsMock;

	private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

	private readonly Mock<ISymmetricCryptoService> _cryptoServiceMock;

	private readonly ISetup<ISymmetricCryptoService, Task<string>> _decryptionSetup;

	private readonly ISetup<IOptions<AccessTokenOptions>, AccessTokenOptions> _optionsSetup;

	public AccessTokenManagerUnitTests()
	{
		_optionsMock = new Mock<IOptions<AccessTokenOptions>>();
		_optionsSetup = _optionsMock.Setup(o => o.Value);
		_optionsSetup.Returns(new AccessTokenOptions
		{
			Key = "1111111111111111",
			Issuer = "issuer",
			Audience = "audience",
			LifeSpan = TimeSpan.FromHours(1)
		});

		var realCryptoService = new AesSymmetricCryptoService();
		_cryptoServiceMock = new Mock<ISymmetricCryptoService>();

		_cryptoServiceMock
			.Setup(s => s.EncryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
			.Returns((string plainText, byte[] key, CancellationToken ct) =>
				realCryptoService.EncryptAsync(plainText, key, ct));

		_decryptionSetup = _cryptoServiceMock.Setup(s => s.DecryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));
		_decryptionSetup.Returns((string plainText, byte[] key, CancellationToken ct) =>
			realCryptoService.DecryptAsync(plainText, key, ct));

		_dateTimeProviderMock = new Mock<IDateTimeProvider>();
		_dateTimeProviderMock.Setup(p => p.UtcNow).Returns(DateTime.UtcNow);

		_accessTokenManager = new AccessTokenManager(
			_optionsMock.Object,
			NullLogger<AccessTokenManager>.Instance,
			_cryptoServiceMock.Object,
			_dateTimeProviderMock.Object);
	}

	[Fact]
	public async Task CreateMeaningfulEncryptedToken()
	{
		var userId = UserId.Parse("DA50CA0F-3A82-4CB5-A83E-707C7C875A49");
		var token = await _accessTokenManager.CreateEncrypted(userId, CancellationToken.None);
		token.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public void ReturnSuccessfulValidation()
	{
		var accessToken = new AccessToken
		{
			UserId = UserId.Parse("3EFD2C42-3E00-40F7-BB23-E25C8B9103BD"),
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(1),
			Issuer = "issuer",
			Audience = "audience",
		};

		var validationResult = _accessTokenManager.Validate(accessToken);

		validationResult.IsSuccess.Should().BeTrue();
	}

	[Fact]
	public void ReturnFailureValidation_WhenTokenHasExpired()
	{
		var accessToken = new AccessToken()
		{
			ExpiresAt = DateTime.UtcNow.AddHours(-1),
			Issuer = "issuer",
			Audience = "audience"
		};

		var validationResult = _accessTokenManager.Validate(accessToken);

		validationResult.IsFailure.Should().BeTrue();
		validationResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public void ReturnFailureValidation_WhenIssuerIsInvalid()
	{
		var accessToken = new AccessToken()
		{
			ExpiresAt = DateTime.UtcNow.AddHours(1),
			Issuer = "INVALID",
			Audience = "audience"
		};

		var validationResult = _accessTokenManager.Validate(accessToken);

		validationResult.IsFailure.Should().BeTrue();
		validationResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public void ReturnFailureValidation_WhenAudienceIsInvalid()
	{
		var accessToken = new AccessToken()
		{
			ExpiresAt = DateTime.UtcNow.AddHours(1),
			Issuer = "issuer",
			Audience = "INVALID"
		};

		var validationResult = _accessTokenManager.Validate(accessToken);

		validationResult.IsFailure.Should().BeTrue();
		validationResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public async Task SuccessfullyDecryptToken()
	{
		var userId = UserId.Parse("74A42F0A-CDA7-4E57-95F6-FA6B0CC7EDD5");
		var token = await _accessTokenManager.CreateEncrypted(userId, CancellationToken.None);

		var decryptionResult = await _accessTokenManager.Decrypt(token, CancellationToken.None);
		decryptionResult.IsSuccess.Should().BeTrue();
		decryptionResult.Value.UserId.Should().Be(userId);
	}

	[Fact]
	public async Task ReturnFailureDecryption_WhenInvalidKey()
	{
		var userId = UserId.Parse("E72629CF-0BFF-4762-92EE-D112ADCB6A73");
		var token = await _accessTokenManager.CreateEncrypted(userId, CancellationToken.None);

		_optionsSetup.Returns(new AccessTokenOptions
		{
			Key = "INVALID_KEY12345",
			Issuer = "issuer",
			Audience = "audience",
			LifeSpan = TimeSpan.FromHours(1)
		});
		var newTokenManager = new AccessTokenManager(
			_optionsMock.Object,
			NullLogger<AccessTokenManager>.Instance,
			_cryptoServiceMock.Object,
			_dateTimeProviderMock.Object);

		var decryptionResult = await newTokenManager.Decrypt(token, CancellationToken.None);
		decryptionResult.IsFailure.Should().BeTrue();
		decryptionResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}

	[Fact]
	public async Task ReturnFailureDecryption_WhenCannotConvertJsonToToken()
	{
		_decryptionSetup.Returns(Task.FromResult("{}")); // empty json

		var decryptionResult = await _accessTokenManager.Decrypt(string.Empty, CancellationToken.None);
		decryptionResult.IsFailure.Should().BeTrue();
		decryptionResult.Error.Type.Should().Be(ErrorType.Unauthorized);
	}
}
