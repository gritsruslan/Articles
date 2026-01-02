namespace Articles.Application.Interfaces.Security;

public interface IPasswordHasher
{
	byte[] GenerateSalt();

	byte[] HashPassword(string password, byte[] salt);

	bool VerifyPassword(byte[] passwordHash, byte[] salt, string password);
}
