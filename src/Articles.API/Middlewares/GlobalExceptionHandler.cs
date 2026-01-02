namespace Articles.API.Middlewares;

internal sealed class GlobalExceptionHandler(RequestDelegate next)
{
	public async Task InvokeAsync(
		HttpContext httpContext,
		ILogger<GlobalExceptionHandler> logger,
		IWebHostEnvironment environment)
	{
		try
		{
			await next(httpContext);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Exception has happened with {RequestPath}, the message is {ExceptionMessage}!",
				httpContext.Request.Path, ex.Message);
			httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

			if (environment.IsDevelopment())
			{
				await httpContext.Response.WriteAsync($"Unhandled exception occured! Exception : {ex}");
				return;
			}

			await httpContext.Response.WriteAsJsonAsync(new
			{
				message = "Unhandled error occured. Please contact us!"
			});
		}
	}
}
