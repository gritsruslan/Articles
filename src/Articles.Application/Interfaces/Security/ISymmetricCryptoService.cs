namespace Articles.Application.Interfaces.Security;

public interface ISymmetricCryptoService
{
	Task<string> EncryptAsync(string plainText, byte[] key, CancellationToken cancellationToken);

	Task<string> DecryptAsync(string plainText, byte[] key, CancellationToken cancellationToken);
}
