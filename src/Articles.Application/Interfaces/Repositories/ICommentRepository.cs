namespace Articles.Application.Interfaces.Repositories;

public interface ICommentRepository
{
	Task Add(Comment comment, CancellationToken cancellationToken);
}
