using Articles.Shared.Abstraction;
using Articles.Shared.DefaultServices;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Articles.Storage.Postgres.Interceptors;

internal sealed class AuditableEntityInterceptor(
	IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = new())
	{
		var dbContext = eventData.Context;

		if (dbContext is not null)
		{
			foreach (var entry in dbContext.ChangeTracker.Entries<BaseAuditableEntity>())
			{
				var dateTime = dateTimeProvider.UtcNow;
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = dateTime;
				}
				else if (entry.State == EntityState.Modified)
				{
					entry.Entity.UpdatedAt = dateTime;
				}
			}
		}

		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}
}
