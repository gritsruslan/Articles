namespace Articles.Storage.Postgres.Tests;

public class SessionRepositoryFixture : StorageTestsFixture
{
	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		await using var dbContext = GetDbContext();

		var emptyHash = Enumerable.Repeat<byte>(0, SecurityConstants.PasswordHashLength).ToArray();
		var emptySalt = Enumerable.Repeat<byte>(0, SecurityConstants.SaltLength).ToArray();
		dbContext.Users.AddRange(new UserEntity
		{
			Id = Guid.Parse("F0EFDC5D-CB9E-4EEA-8ED2-58D122B646DD"),
			Email = "sessionsTest1@gmail.com",
			Name = "name",
			DomainId = "sessionsTest1",
			PasswordHash = emptyHash,
			Salt = emptySalt
		},
		new UserEntity
		{
			Id = Guid.Parse("FF45B20A-2C72-40DE-8D0B-EF1BEBB5085A"),
			Email = "sessionsTest2@gmail.com",
			Name = "name",
			DomainId = "sessionsTest2",
			PasswordHash = emptyHash,
			Salt = emptySalt
		});

		dbContext.Sessions.AddRange(new SessionEntity()
		{
			Id = Guid.Parse("FF96B9EB-5B1D-4E96-A17F-5B5E160106D0"),
			UserId = Guid.Parse("F0EFDC5D-CB9E-4EEA-8ED2-58D122B646DD"),
			UserAgent = "Windows 52",
			IssuedAt = new DateTime(2025, 11, 03, 18, 44, 52, DateTimeKind.Utc),
			ExpiresAt = new DateTime(2025, 11, 03, 18, 44, 52, DateTimeKind.Utc)
		},
		new SessionEntity
		{
			Id = Guid.Parse("FB58EDF6-759C-41AC-8ACA-69BEE2824991"),
			UserId = Guid.Parse("FF45B20A-2C72-40DE-8D0B-EF1BEBB5085A"),
			UserAgent = "Iphone 52",
			IssuedAt = new DateTime(2025, 11, 03, 18, 44, 52, DateTimeKind.Utc),
			ExpiresAt = new DateTime(2025, 11, 03, 18, 44, 52, DateTimeKind.Utc)
		});

		await dbContext.SaveChangesAsync();
	}
}

public class SessionRepositoryTests(SessionRepositoryFixture fixture) : IClassFixture<SessionRepositoryFixture>
{
	private readonly ISessionRepository _repository = new SessionRepository(fixture.GetDbContext());

	[Fact]
	public async Task SuccessfulAddSession()
	{
		var newSession = new Session()
		{
			Id = SessionId.Parse("BD35B2C5-5488-4487-981D-855AA5373D54"),
			UserId = UserId.Parse("F0EFDC5D-CB9E-4EEA-8ED2-58D122B646DD"),
			UserAgent = "Windows 52",
			IssuedAt = new DateTime(2025, 11, 3, 18, 44, 52, DateTimeKind.Utc),
			ExpiresAt = new DateTime(2025, 11, 3, 18, 44, 52, DateTimeKind.Utc)
		};

		await _repository.Add(newSession, CancellationToken.None);

		await using var dbContext = fixture.GetDbContext();
		var session = dbContext.Sessions.FirstOrDefault(s => s.Id == newSession.Id.Value);

		session.Should().NotBeNull();
		session.UserId.Should().Be(newSession.UserId.Value);
		session.UserAgent.Should().Be(newSession.UserAgent);
		session.IssuedAt.Should().Be(newSession.IssuedAt);
		session.ExpiresAt.Should().Be(newSession.ExpiresAt);
	}

	[Fact]
	public async Task ReturnSession_WhenSessionWithSameIdExists()
	{
		var sessionId = SessionId.Parse("FF96B9EB-5B1D-4E96-A17F-5B5E160106D0");
		var session = await _repository.GetById(sessionId, CancellationToken.None);
		session.Should().NotBeNull();
	}

	[Fact]
	public async Task ReturnNull_WhenSessionWithSameIdNotExists()
	{
		var sessionId = SessionId.Parse("0D298077-EDB1-4C30-A9E7-5B7F9FD14D57");
		var session = await _repository.GetById(sessionId, CancellationToken.None);
		session.Should().BeNull();
	}

	[Fact]
	public async Task SuccessfulDeleteSession()
	{
		var sessionId = SessionId.Parse("FB58EDF6-759C-41AC-8ACA-69BEE2824991");

		await _repository.DeleteById(sessionId, CancellationToken.None);

		await using var dbContext = fixture.GetDbContext();
		var sessionExists = await dbContext.Sessions.AnyAsync(s => s.Id == sessionId.Value);
		sessionExists.Should().BeFalse();
	}
}
