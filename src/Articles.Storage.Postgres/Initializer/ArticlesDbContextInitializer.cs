using Articles.Application.Interfaces.Security;
using Articles.Domain.Enums;
using Articles.Shared.Options;
using Articles.Storage.Postgres.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Articles.Storage.Postgres.Initializer;

internal sealed class ArticlesDbContextInitializer(
	ArticlesDbContext dbContext,
	ILogger<ArticlesDbContextInitializer> logger,
	IOptions<SupervisorUserOptions> supervisorUserOptions,
	IPasswordHasher passwordHasher) : IDbContextInitializer
{
	public async Task MigrateAsync()
	{
		try
		{
			await dbContext.Database.MigrateAsync();
			logger.LogInformation("Database migrated successfully");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while initializing the database");
			throw;
		}
	}

	public async Task SeedAsync()
	{
		var options = supervisorUserOptions.Value;

		if (await dbContext.Users.AnyAsync(u => u.Id == options.Id))
		{
			return;
		}

		var salt = passwordHasher.GenerateSalt();
		var passwordHash = passwordHasher.HashPassword(options.Password, salt);

		var supervisorUser = new UserEntity
		{
			Id = options.Id,
			Email = options.Email,
			Name = options.Name,
			RoleId = (int)Roles.Admin,
			DomainId = options.DomainId,
			EmailConfirmed = true,
			PasswordHash = passwordHash,
			Salt = salt
		};

		dbContext.Users.Add(supervisorUser);
		await dbContext.SaveChangesAsync();
	}
}
