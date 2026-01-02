using Articles.Domain.Errors;
using Articles.Infrastructure.Mail;
using Articles.Infrastructure.Security;
using Articles.Shared.DefaultServices;
using Articles.Shared.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Articles.UnitTests.Authentication;

public class EmailConfirmationTokenManagerUnitTests
{
	private readonly IEmailConfirmationTokenManager _emailTokenManager;

	private readonly Mock<IOptions<EmailConfirmationTokenOptions>> _optionsMock;

	private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

	private readonly Mock<ISymmetricCryptoService> _cryptoServiceMock;

	private readonly ISetup<ISymmetricCryptoService, Task<string>> _decryptionSetup;

	private readonly ISetup<IOptions<EmailConfirmationTokenOptions>, EmailConfirmationTokenOptions> _optionsSetup;

	public EmailConfirmationTokenManagerUnitTests()
	{
		_dateTimeProviderMock = new Mock<IDateTimeProvider>();
		_dateTimeProviderMock.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

		var realCryptoService = new AesSymmetricCryptoService();
		_cryptoServiceMock = new Mock<ISymmetricCryptoService>();
		_cryptoServiceMock
			.Setup(s => s.EncryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
			.Returns((string plainText, byte[] key, CancellationToken ct) =>
				realCryptoService.EncryptAsync(plainText, key, ct));

		_decryptionSetup = _cryptoServiceMock.Setup(s => s.DecryptAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));
		_decryptionSetup.Returns((string plainText, byte[] key, CancellationToken ct) =>
			realCryptoService.DecryptAsync(plainText, key, ct));

		_optionsMock = new Mock<IOptions<EmailConfirmationTokenOptions>>();
		_optionsSetup = _optionsMock.Setup(s => s.Value);
		_optionsSetup.Returns(new EmailConfirmationTokenOptions()
		{
			Key = "1111111111111111",
			LifeSpan = TimeSpan.FromHours(1)
		});

		_emailTokenManager = new EmailConfirmationTokenManager(
			_cryptoServiceMock.Object,
			_optionsMock.Object,
			_dateTimeProviderMock.Object,
			NullLogger<EmailConfirmationTokenManager>.Instance);
	}

	[Fact]
	public async Task ReturnMeaningfulToken()
	{
		var token = new EmailConfirmationToken
		{
			UserId = UserId.Parse("38C2B22B-20F9-4BCD-B013-60724C8131D3"),
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(1)
		};

		var encrypted = await _emailTokenManager.EncryptToken(token, CancellationToken.None);
		encrypted.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public void SuccessfullyValidateToken()
	{
		var token = new EmailConfirmationToken
		{
			UserId = UserId.Parse("9CCC1FAB-0AAF-4FCD-9AB9-3F85A8EC3596"),
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(1)
		};

		var validationResult = _emailTokenManager.Validate(token);
		validationResult.IsSuccess.Should().BeTrue();
	}

	[Fact]
	public void ReturnFailureValidation_WhenTokenHasExpired()
	{
		var token = new EmailConfirmationToken
		{
			UserId = UserId.Parse("BD7850BA-00C2-431E-A244-07040673FF2D"),
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(-1)
		};

		var validationResult = _emailTokenManager.Validate(token);
		validationResult.IsFailure.Should().BeTrue();
		validationResult.Error.Should().BeEquivalentTo(SecurityErrors.EmailConfirmationTokenExpired());
	}

	[Fact]
	public async Task SuccessfullyDecryptToken()
	{
		var token = new EmailConfirmationToken
		{
			UserId = UserId.Parse("89A77745-5230-4FBA-B3A1-B13A503668C2"),
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(1)
		};

		var encrypted = await _emailTokenManager.EncryptToken(token, CancellationToken.None);

		var decryptionResult = await _emailTokenManager.DecryptToken(encrypted, CancellationToken.None);
		decryptionResult.IsSuccess.Should().BeTrue();
		decryptionResult.Value.Should().BeEquivalentTo(token);
	}

	[Fact]
	public async Task ReturnFailureDecryption_WhenInvalidKey()
	{
		var token = new EmailConfirmationToken
		{
			UserId = UserId.Parse("AF5C6D48-6CF7-4BB9-9800-B40E62C363D4"),
			IssuedAt = DateTime.UtcNow,
			ExpiresAt = DateTime.UtcNow.AddHours(1)
		};
		var encrypted = await _emailTokenManager.EncryptToken(token, CancellationToken.None);
		_optionsSetup.Returns(new EmailConfirmationTokenOptions
		{
			Key = "INVALID_KEY____",
			LifeSpan = TimeSpan.FromHours(1)
		});

		var newEmailTokenManager = new EmailConfirmationTokenManager(
			_cryptoServiceMock.Object,
			_optionsMock.Object,
			_dateTimeProviderMock.Object,
			NullLogger<EmailConfirmationTokenManager>.Instance);

		var decryptionResult = await newEmailTokenManager.DecryptToken(encrypted, CancellationToken.None);
		decryptionResult.IsFailure.Should().BeTrue();
		decryptionResult.Error.Should().BeEquivalentTo(SecurityErrors.InvalidEmailConfirmationToken());
	}

	[Fact]
	public async Task ReturnFailureDecryption_WhenCannotConvertJsonToToken()
	{
		_decryptionSetup.Returns(Task.FromResult("{}")); // empty json

		var decryptionResult = await _emailTokenManager.DecryptToken(string.Empty, CancellationToken.None);
		decryptionResult.IsFailure.Should().BeTrue();
		decryptionResult.Error.Should().BeEquivalentTo(SecurityErrors.InvalidEmailConfirmationToken());
	}
}
