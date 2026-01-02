using Articles.Domain.ValueObjects;

namespace Articles.Storage.Postgres.Tests;


public class UserRepositoryClassFixture : StorageTestsFixture
{
	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		await using var dbContext = GetDbContext();

		var emptyHash = Enumerable.Repeat<byte>(0, SecurityConstants.PasswordHashLength).ToArray();
		var emptySalt = Enumerable.Repeat<byte>(0, SecurityConstants.SaltLength).ToArray();

		dbContext.Users.AddRange(new UserEntity
		{
			Id = Guid.Parse("DD0A7F28-0B68-4AF3-A29D-366FBE84D1C4"),
			Email = "testUser@gmail.com",
			Name = "TEST USER",
			DomainId = "TestUser123",
			PasswordHash = emptyHash,
			Salt = emptySalt
		});
		await dbContext.SaveChangesAsync();
	}
}

public class UserRepositoryTests(UserRepositoryClassFixture fixture) : IClassFixture<UserRepositoryClassFixture>
{
	private readonly IUserRepository _repository = new UserRepository(fixture.GetDbContext());

	[Fact]
	public async Task ReturnTrue_WhenUserWithSameDomainIdExists()
	{
		var result = await _repository.ExistsByDomainId(
			DomainId.CreateVerified("TestUser123"), CancellationToken.None);
		result.Should().BeTrue();
	}

	[Fact]
	public async Task ReturnFalse_WhenUserWithSameDomainIdNotExists()
	{
		var result = await _repository.ExistsByDomainId(
			DomainId.CreateVerified("invalid"), CancellationToken.None);
		result.Should().BeFalse();
	}

	[Fact]
	public async Task ReturnTrue_WhenUserWithSameEmailExists()
	{
		var result = await _repository.ExistsByEmail(
			Email.CreateVerified("testUser@gmail.com"), CancellationToken.None);
		result.Should().BeTrue();
	}

	[Fact]
	public async Task ReturnFalse_WhenUserWithSameEmailNotExists()
	{
		var result = await _repository.ExistsByDomainId(
			DomainId.CreateVerified("invalid@email.ha"), CancellationToken.None);
		result.Should().BeFalse();
	}

	[Fact]
	public async Task ReturnUser_WhenUserWithSameEmailExists()
	{
		var email = "testUser@gmail.com";
		var user = await _repository.GetByEmail(Email.CreateVerified(email), CancellationToken.None);
		user.Should().NotBeNull();
		user.Email.Value.Should().Be(email);
	}

	[Fact]
	public async Task ReturnNull_WhenUserWithSameEmailNotExists()
	{
		var user = await _repository.GetByEmail(Email.CreateVerified("invalid@gmail.com"), CancellationToken.None);
		user.Should().BeNull();
	}

	[Fact]
	public async Task SuccessfullyAddUser()
	{
		var emptyHash = Enumerable.Repeat<byte>(0, SecurityConstants.PasswordHashLength).ToArray();
		var emptySalt = Enumerable.Repeat<byte>(0, SecurityConstants.SaltLength).ToArray();

		var userId = UserId.New();
		var email = "newuser@email.com";
		var name = "any";
		var domainId = "anyid123";
		var newUser = new User
		{
			Id = userId,
			Email = Email.CreateVerified(email),
			Name = UserName.CreateVerified(name),
			DomainId = DomainId.CreateVerified(domainId),
			PasswordHash = emptyHash,
			Salt = emptySalt
		};

		await _repository.Add(newUser, CancellationToken.None);

		await using var dbContext = fixture.GetDbContext();
		var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);

		user.Should().NotBeNull();
		user.Id.Should().Be(userId.Value);
		user.Email.Should().Be(email);
		user.Name.Should().Be(name);
		user.DomainId.Should().Be(domainId);
	}

	[Fact]
	public async Task SuccessfullyConfirmEmail()
	{
		var userId = Guid.Parse("DD0A7F28-0B68-4AF3-A29D-366FBE84D1C4");
		await _repository.ConfirmEmail(UserId.Create(userId), CancellationToken.None);

		await using var dbContext = fixture.GetDbContext();
		var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

		user.Should().NotBeNull().And.Subject.As<UserEntity>()
			.EmailConfirmed.Should().BeTrue();
	}
}
