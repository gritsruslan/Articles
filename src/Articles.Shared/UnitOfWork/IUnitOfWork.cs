namespace Articles.Shared.UnitOfWork;

public interface IUnitOfWork
{
	Task<IUnitOfWorkScope> StartScope(CancellationToken cancellationToken);
}
