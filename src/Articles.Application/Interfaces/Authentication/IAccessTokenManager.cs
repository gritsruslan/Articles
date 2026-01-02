using Articles.Application.Authentication;
using Articles.Domain.Identifiers;
using Articles.Shared.Result;

namespace Articles.Application.Interfaces.Authentication;

public interface IAccessTokenManager
{
	Task<string> CreateEncrypted(UserId userId, CancellationToken cancellationToken);

	Result Validate(AccessToken accessToken);

	Task<Result<AccessToken>> Decrypt(string accessTokenStr, CancellationToken cancellationToken);
}
