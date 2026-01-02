using Articles.Shared.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace Articles.Storage.Postgres.UnitOfWork;

internal sealed class UnitOfWorkScope(IDbContextTransaction transaction) : IUnitOfWorkScope
{
	public async Task Commit(CancellationToken cancellationToken) =>
		await transaction.CommitAsync(cancellationToken);

	public async ValueTask DisposeAsync() =>
		await transaction.DisposeAsync();
}
