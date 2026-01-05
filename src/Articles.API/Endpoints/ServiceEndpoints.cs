using Articles.API.Requests;
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

	private static IResult Test([FromServices] ILoggerFactory loggerFactory)
	{
		var logger = loggerFactory.CreateLogger("Test");

		logger.LogInformation("INFORMATION TEST LOG");
		logger.LogWarning("WARNING TEST LOG");
		logger.LogError("ERROR TEST LOG");

		return Results.Ok();
	}
}
