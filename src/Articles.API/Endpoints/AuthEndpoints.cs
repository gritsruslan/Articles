using Articles.API.Authentication;
using Articles.API.Extensions;
using Articles.API.Handlers;
using Articles.API.Requests;
using Articles.Application.UseCases.Auth.ConfirmEmail;
using Articles.Application.UseCases.Auth.Login;
using Articles.Application.UseCases.Auth.Logout;
using Articles.Application.UseCases.Auth.Me;
using Articles.Application.UseCases.Auth.RefreshTokens;
using Articles.Application.UseCases.Auth.Registration;
using Articles.Shared.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Articles.API.Endpoints;

internal static class AuthEndpoints
{
	public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("auth")
			.RequireRateLimiting(SecurityExtensions.AuthRateLimitingPolicy);

		group.MapPost("login", Login);
		group.MapPost("registration", Registration);
		group.MapPost("logout", Logout);
		group.MapGet("confirm-email", ConfirmEmail);
		group.MapPost("refresh", RefreshTokens);
		group.MapGet("me", Me);

		return app;
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
	private static async Task<IResult> Login(
		[FromBody] LoginRequest request,
		[FromServices] GlobalCommandHandler handler,
		[FromServices] IAuthTokenStorage tokenStorage,
		HttpContext httpContext,
		CancellationToken cancellationToken)
	{
		var command = new LoginCommand(
			request.Email,
			request.Password,
			request.RememberMe,
			tokenStorage.GetUserAgent());

		return await handler.Handle(command, result =>
		{
			tokenStorage.StoreAccessToken(result.AccessToken);
			tokenStorage.StoreRefreshToken(result.RefreshToken);
			return Results.Ok();
		}, cancellationToken);
	}

	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
	private static async Task<IResult> Registration(
		[FromBody] RegistrationRequest request,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new RegistrationCommand(request.Name, request.Email, request.DomainId, request.Password);
		return await handler.Handle(command, Results.Created, cancellationToken);
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
	private static async Task<IResult> ConfirmEmail(
		[FromQuery] string token,
		[FromServices] GlobalCommandHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new ConfirmEmailCommand(token);
		return await handler.Handle(command, Results.Ok, cancellationToken);
	}

	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
	private static async Task<IResult> Logout(
		[FromServices] GlobalCommandHandler handler,
		[FromServices] IAuthTokenStorage tokenStorage,
		CancellationToken cancellationToken)
	{
		var command = new LogoutCommand(tokenStorage.GetRefreshToken());
		return await handler.Handle(command, _ =>
		{
			tokenStorage.RemoveAccessToken();
			tokenStorage.RemoveRefreshToken();
			return Results.NoContent();
		}, cancellationToken);
	}


	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
	private static async Task<IResult> RefreshTokens(
		[FromServices] GlobalCommandHandler handler,
		[FromServices] IAuthTokenStorage tokenStorage,
		CancellationToken cancellationToken)
	{
		var command = new RefreshTokensCommand(
			tokenStorage.GetRefreshToken(),
			tokenStorage.GetUserAgent());

		return await handler.Handle(command, authTokenPair =>
		{
			tokenStorage.StoreAccessToken(authTokenPair.AccessToken);
			tokenStorage.StoreRefreshToken(authTokenPair.RefreshToken);
			return Results.Ok();
		}, cancellationToken);
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	private static async Task<IResult> Me(
		[FromServices] GlobalQueryHandler handler,
		CancellationToken cancellationToken)
	{
		return await handler.Handle(new MeQuery(), Results.Ok, cancellationToken);
	}
}
