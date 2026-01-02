using System.Security.Cryptography;
using Articles.Infrastructure.Security;

namespace Articles.UnitTests.Security;

public class AesSymmetricCryptoServiceUnitTests
{
	private readonly ISymmetricCryptoService _cryptoService = new AesSymmetricCryptoService();

	[Fact]
	public async Task ReturnMeaningfulEncryptedString()
	{
		var plainText = "Hello World";
		var encrypted = await _cryptoService.EncryptAsync(
			plainText, RandomNumberGenerator.GetBytes(16), CancellationToken.None);
		encrypted.Should().NotBeEmpty();
	}

	[Fact]
	public async Task DecryptEncryptedString_WhenKeyIsTheSame()
	{
		var key = RandomNumberGenerator.GetBytes(16);
		var plainText = "Hello, world!";
		var encrypted = await _cryptoService.EncryptAsync(plainText, key, CancellationToken.None);
		var decrypted = await _cryptoService.DecryptAsync(encrypted, key, CancellationToken.None);
		decrypted.Should().Be(plainText);
	}

	[Fact]
	public async Task ThrowException_WhenDecryptingWithTheDifferentKey()
	{
		var encrypted = await _cryptoService.EncryptAsync(
			"Hello, world!", RandomNumberGenerator.GetBytes(16), CancellationToken.None);
		await _cryptoService.Invoking(s =>
				s.DecryptAsync(encrypted, RandomNumberGenerator.GetBytes(16), CancellationToken.None))
			.Should().ThrowAsync<CryptographicException>();
	}
}
