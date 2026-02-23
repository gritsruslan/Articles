using Articles.API.Authentication;
using Articles.API.Extensions;
using Articles.API.Requests;
using Articles.Application.AuthUseCases.Commands.ConfirmEmail;
using Articles.Application.AuthUseCases.Commands.Login;
using Articles.Application.AuthUseCases.Commands.Logout;
using Articles.Application.AuthUseCases.Commands.Me;
using Articles.Application.AuthUseCases.Commands.RefreshTokens;
using Articles.Application.AuthUseCases.Commands.Registration;
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
		[FromServices] ISender sender,
		[FromServices] IAuthTokenStorage tokenStorage,
		HttpContext httpContext,
		CancellationToken cancellationToken)
	{
		var command = new LoginCommand(
			request.Email,
			request.Password,
			request.RememberMe,
			tokenStorage.GetUserAgent());

		var result = await sender.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		tokenStorage.StoreAccessToken(result.Value.AccessToken);
		tokenStorage.StoreRefreshToken(result.Value.RefreshToken);

		return Results.Ok();
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
	private static async Task<IResult> Registration(
		[FromBody] RegistrationRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new RegistrationCommand(request.Name, request.Email, request.DomainId, request.Password);

		var result = await sender.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Created();
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
	private static async Task<IResult> ConfirmEmail(
		[FromQuery] string token,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var command = new ConfirmEmailCommand(token);

		var result = await sender.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok();
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
	private static async Task<IResult> Logout(
		[FromServices] ISender sender,
		[FromServices] IAuthTokenStorage tokenStorage,
		CancellationToken cancellationToken)
	{
		var command = new LogoutCommand(tokenStorage.GetRefreshToken());

		var result = await sender.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		tokenStorage.RemoveAccessToken();
		tokenStorage.RemoveRefreshToken();

		return Results.NoContent();
	}


	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
	private static async Task<IResult> RefreshTokens(
		[FromServices] ISender sender,
		[FromServices] IAuthTokenStorage tokenStorage,
		CancellationToken cancellationToken)
	{
		var command = new RefreshTokensCommand(
			tokenStorage.GetRefreshToken(),
			tokenStorage.GetUserAgent());

		var result = await sender.Send(command, cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		tokenStorage.StoreAccessToken(result.Value.AccessToken);
		tokenStorage.StoreRefreshToken(result.Value.RefreshToken);

		return Results.Ok();
	}

	[ProducesResponseType(StatusCodes.Status200OK)]
	private static async Task<IResult> Me(
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new MeQuery(), cancellationToken);
		if (result.IsFailure)
		{
			return result.Error.ToResponse();
		}

		return Results.Ok(result.Value);
	}
}
