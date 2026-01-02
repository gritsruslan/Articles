using System.Security.Cryptography;
using Articles.Domain.Constants;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Articles.Infrastructure.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
	private const int HashIterationsCount = 1000;

	public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SecurityConstants.SaltLength);

	public byte[] HashPassword(string password, byte[] salt)
	{
		return KeyDerivation.Pbkdf2(
			password,
			salt,
			KeyDerivationPrf.HMACSHA256,
			HashIterationsCount,
			SecurityConstants.PasswordHashLength);
	}

	public bool VerifyPassword(byte[] passwordHash, byte[] salt, string password) =>
		HashPassword(password, salt).SequenceEqual(passwordHash);
}
