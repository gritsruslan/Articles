using Articles.Application.Authentication;
using Articles.Domain.Identifiers;

namespace Articles.Application.Interfaces.Authentication;

public interface IAuthenticationService
{
	Task<RecognizedUser> Authenticate(UserId userId, CancellationToken cancellationToken);
}
