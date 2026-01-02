using Articles.Shared.UnitOfWork;

namespace Articles.Storage.Postgres.UnitOfWork;

internal sealed class UnitOfWork(ArticlesDbContext dbContext) : IUnitOfWork
{
	public async Task<IUnitOfWorkScope> StartScope(CancellationToken cancellationToken)
	{
		var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
		return new UnitOfWorkScope(transaction);
	}
}
