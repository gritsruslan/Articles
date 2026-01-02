using Articles.Domain.Identifiers;
using Articles.Domain.Models;
using Articles.Domain.ValueObjects;

namespace Articles.Application.Interfaces.Repositories;

public interface IUserRepository
{
	Task<bool> ExistsByDomainId(DomainId domainId, CancellationToken cancellationToken);

	Task<bool> ExistsByEmail(Email email, CancellationToken cancellationToken);

	Task<User?> GetByEmail(Email email, CancellationToken cancellationToken);

	Task<User?> GetById(UserId id, CancellationToken cancellationToken);

	Task Add(User user, CancellationToken cancellationToken);

	Task ConfirmEmail(UserId userId, CancellationToken cancellationToken);
}
