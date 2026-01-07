using Articles.API.Requests;
using Articles.Application.Interfaces.Repositories;
using Articles.Domain.DomainEvents;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Serilog.Events;

namespace Articles.API.Endpoints;

internal static class ServiceEndpoints
{
	public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("service");

		group.MapGet("currentLogLevel", CurrentLogLevel);
		group.MapPost("switchLogLevel", SwitchLogLevel);
		group.MapGet("test", Test);

		return app;
	}

	private static IResult CurrentLogLevel([FromServices] LoggingLevelSwitch loggingLevelSwitch)
	{
		return Results.Ok(new { CurrentLogLevel = loggingLevelSwitch.MinimumLevel.ToString() });
	}

	private static IResult SwitchLogLevel(
		[FromBody] SwitchLogLevelRequest request,
		[FromServices] LoggingLevelSwitch loggingLevelSwitch)
	{
		loggingLevelSwitch.MinimumLevel = request.NewMinimumLevel.ToLower() switch
		{
			"debug" => LogEventLevel.Debug,
			"info" => LogEventLevel.Information,
			"warning" => LogEventLevel.Warning,
			"error" => LogEventLevel.Error,
			"fatal" => LogEventLevel.Fatal,
			_ => LogEventLevel.Debug
		};

		return  Results.Ok(new { CurrentLogLevel = loggingLevelSwitch.MinimumLevel.ToString() });
	}

	private static async Task<IResult> Test(
		[FromServices] ILoggerFactory loggerFactory,
		[FromServices] IDomainEventRepository repository)
	{
		var @event = new TestDomainEvent(Guid.NewGuid());
		await repository.Add(@event, CancellationToken.None);

		return Results.Ok();
	}
}
