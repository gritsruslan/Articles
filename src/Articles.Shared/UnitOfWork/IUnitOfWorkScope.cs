namespace Articles.Shared.UnitOfWork;

public interface IUnitOfWorkScope : IAsyncDisposable
{
	Task Commit(CancellationToken cancellationToken);
}
