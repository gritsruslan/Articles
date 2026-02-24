using Articles.API.Requests;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Serilog.Events;

namespace Articles.API.Endpoints;

// only for testing
internal static class ServiceEndpoints
{
	public static IEndpointRouteBuilder MapServiceEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("service");

		group.MapGet("currentLogLevel", CurrentLogLevel);
		group.MapPost("switchLogLevel", SwitchLogLevel);

		return app;
	}

	// only for testing
	private static IResult CurrentLogLevel([FromServices] LoggingLevelSwitch loggingLevelSwitch)
	{
		return Results.Ok(new { CurrentLogLevel = loggingLevelSwitch.MinimumLevel.ToString() });
	}

	// only for testing
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
}
