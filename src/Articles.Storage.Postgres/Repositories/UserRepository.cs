using Articles.Domain.ValueObjects;
using Articles.Storage.Postgres.Entities;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class UserRepository(ArticlesDbContext dbContext) : IUserRepository
{
	public Task<bool> ExistsByDomainId(DomainId domainId, CancellationToken cancellationToken) =>
		dbContext.Users.AnyAsync(u => u.DomainId == domainId.Value, cancellationToken);

	public Task<bool> ExistsByEmail(Email email, CancellationToken cancellationToken) =>
		dbContext.Users.AnyAsync(u => u.Email == email.Value, cancellationToken);

	public async Task<User?> GetByEmail(Email email, CancellationToken cancellationToken)
	{
		var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.Value, cancellationToken);
		return user is null
			? null
			: new User
			{
				Id = UserId.Create(user.Id),
				Name = UserName.CreateVerified(user.Name),
				Email = Email.CreateVerified(user.Email),
				RoleId = RoleId.Create(user.RoleId),
				DomainId = DomainId.CreateVerified(user.DomainId),
				EmailConfirmed = user.EmailConfirmed,
				PasswordHash = user.PasswordHash,
				Salt = user.Salt
			};
	}

	public async Task<User?> GetById(UserId id, CancellationToken cancellationToken)
	{
		var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id.Value, cancellationToken);
		return user is null
			? null
			: new User
			{
				Id = UserId.Create(user.Id),
				Name = UserName.CreateVerified(user.Name),
				Email = Email.CreateVerified(user.Email),
				RoleId = RoleId.Create(user.RoleId),
				DomainId = DomainId.CreateVerified(user.DomainId),
				EmailConfirmed = user.EmailConfirmed,
				PasswordHash = user.PasswordHash,
				Salt = user.Salt
			};
	}

	public Task Add(User user, CancellationToken cancellationToken)
	{
		var userEntity = new UserEntity
		{
			Id = user.Id.Value,
			Email = user.Email.Value,
			Name = user.Name.Value,
			DomainId = user.DomainId.Value,
			EmailConfirmed = user.EmailConfirmed,
			RoleId = user.RoleId.Value,
			PasswordHash = user.PasswordHash,
			Salt = user.Salt
		};

		dbContext.Users.Add(userEntity);
		return dbContext.SaveChangesAsync(cancellationToken);
	}

	public Task ConfirmEmail(UserId userId, CancellationToken cancellationToken) =>
		dbContext.Users.Where(u => u.Id == userId.Value)
			.ExecuteUpdateAsync(u => u.SetProperty(it => it.EmailConfirmed, true), cancellationToken);

	public Task Delete(UserId userId, CancellationToken cancellationToken) =>
		dbContext.Users.Where(u => u.Id == userId.Value).ExecuteDeleteAsync(cancellationToken);
}
