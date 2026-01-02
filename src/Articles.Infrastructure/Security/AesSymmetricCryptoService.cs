using System.Security.Cryptography;
using System.Text;
using Articles.Shared.DefaultServices;

namespace Articles.Infrastructure.Security;

internal sealed class AesSymmetricCryptoService : ISymmetricCryptoService
{
	private const int IvLength = 16;

	public async Task<string> EncryptAsync(string plainText, byte[] key, CancellationToken cancellationToken)
	{
		var iv = RandomNumberGenerator.GetBytes(IvLength);
		var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

		using var aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;

		var encryptor = aes.CreateEncryptor();

		using var memoryStream = new MemoryStream();
		await memoryStream.WriteAsync(iv.AsMemory(0, IvLength), cancellationToken);

		await using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
		{
			await cryptoStream.WriteAsync(plainTextBytes, cancellationToken);
			await cryptoStream.FlushFinalBlockAsync(cancellationToken);
		}

		//return Convert.ToBase64String(memoryStream.ToArray());
		return UrlSafeBase64.ToUrlSafeBase64(memoryStream.ToArray());
	}

	public async Task<string> DecryptAsync(string plainText, byte[] key, CancellationToken cancellationToken)
	{
		//var encryptedBytes = Convert.FromBase64String(plainText);
		var encryptedBytes = UrlSafeBase64.FromUrfSafeBase64(plainText);
		var iv = new byte[IvLength];
		Array.Copy(encryptedBytes, 0, iv, 0, IvLength);

		using var aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;

		var decryptor = aes.CreateDecryptor();

		//encrypted stream
		using var memoryStream = new MemoryStream(encryptedBytes, IvLength, encryptedBytes.Length - IvLength);
		using var resultStream = new MemoryStream();

		await using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
		{
			await cryptoStream.CopyToAsync(resultStream, cancellationToken);
		}

		return Encoding.UTF8.GetString(resultStream.ToArray());
	}
}
