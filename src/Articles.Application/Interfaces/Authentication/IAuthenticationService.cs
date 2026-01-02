using Articles.Application.Authentication;

namespace Articles.Application.Interfaces.Authentication;

public interface IAuthenticationService
{
	Task<RecognizedUser> Authenticate(UserId userId, CancellationToken cancellationToken);
}
