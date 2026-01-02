using Articles.Domain.Constants;
using Articles.Infrastructure.Security;

namespace Articles.UnitTests.Security;

public class PasswordHasherUnitTests
{
	private readonly IPasswordHasher _passwordHasher = new PasswordHasher();

	private static readonly byte[] EmptySalt = Enumerable.Repeat<byte>(0, SecurityConstants.SaltLength).ToArray();
	private static readonly byte[] EmptyHash = Enumerable.Repeat<byte>(0, SecurityConstants.PasswordHashLength).ToArray();

	[Fact]
	public void ReturnMeaningfulHash()
	{
		var password = "mypassword123";
		var salt = _passwordHasher.GenerateSalt();
		var hash = _passwordHasher.HashPassword(password, salt);

		salt.Should().HaveCount(SecurityConstants.SaltLength).And.NotBeEquivalentTo(EmptySalt);
		hash.Should().HaveCount(SecurityConstants.PasswordHashLength).And.NotBeEquivalentTo(EmptyHash);
	}

	[Fact]
	public void ReturnTrue_WhenPasswordAndSaltMatches()
	{
		var password = "!STRON_PASS!";
		var salt = _passwordHasher.GenerateSalt();
		var hash = _passwordHasher.HashPassword(password, salt);

		_passwordHasher.VerifyPassword(hash, salt, password).Should().BeTrue();
	}

	[Fact]
	public void ReturnFalse_WhenPasswordDoesNotMatch()
	{
		var password = "ronaldo17041999";
		var incorrectPassword = "Ronaldo16051998";

		var salt = _passwordHasher.GenerateSalt();
		var hash = _passwordHasher.HashPassword(password, salt);

		_passwordHasher.VerifyPassword(hash, salt, incorrectPassword).Should().BeFalse();
	}

	[Fact]
	public void ReturnFalse_WhenSaltIsInvalid()
	{
		var password = "dsfklgh395434";
		var salt = _passwordHasher.GenerateSalt();
		var hash = _passwordHasher.HashPassword(password, salt);

		_passwordHasher.VerifyPassword(hash, _passwordHasher.GenerateSalt(), password).Should().BeFalse();
	}
}
