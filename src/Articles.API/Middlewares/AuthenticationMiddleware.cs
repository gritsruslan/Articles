using Articles.API.Authentication;
using Articles.Application.Authentication;
using Articles.Application.Interfaces.Authentication;
using Articles.Domain.Identifiers;
using Articles.Shared.DefaultServices;

namespace Articles.API.Middlewares;

internal sealed class AuthenticationMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(
		HttpContext httpContext,
		IAccessTokenManager accessTokenManager,
		IAuthTokenStorage tokenStorage,
		IApplicationUserProvider applicationUserProvider,
		IRoleManager roleManager,
		IAuthenticationService authenticationService,
		IDateTimeProvider dateTimeProvider)
	{
		var accessToken = tokenStorage.GetAccessToken();
		if (accessToken is null)
		{
			SetGuest(applicationUserProvider, roleManager);
			await next(httpContext);
			return;
		}

		var decryptionResult = await accessTokenManager.Decrypt(accessToken, CancellationToken.None);
		if (decryptionResult.IsFailure)
		{
			SetGuest(applicationUserProvider, roleManager);
			await next(httpContext);
			return;
		}
		var token = decryptionResult.Value;
		var validationResult = accessTokenManager.Validate(token);
		if (validationResult.IsFailure)
		{
			SetGuest(applicationUserProvider, roleManager);
			await next(httpContext);
			return;
		}


		var apiUser = await authenticationService.Authenticate(token.UserId, CancellationToken.None);
		applicationUserProvider.CurrentUser = apiUser;

		await next(httpContext);
	}

	private void SetGuest(IApplicationUserProvider applicationUserProvider, IRoleManager roleManager)
	{
		applicationUserProvider.CurrentUser = new RecognizedUser
		{
			Id = UserId.Empty,
			Role = roleManager.Guest()
		};
	}
}
