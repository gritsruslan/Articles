using System.Reflection;
using Microsoft.OpenApi;

namespace Articles.API.Extensions;

internal static class SwaggerExtensions
{
	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Articles.API",
				Version = "v1",
				Description = "Main backend Articles API",
			});

			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			options.IncludeXmlComments(xmlPath);
		});

		return services;
	}

	public static WebApplication UseSwaggerWithUI(this WebApplication app)
	{
		app.UseSwagger();
		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Articles API v1");
			options.RoutePrefix = "swagger";

			options.DisplayRequestDuration();
			options.EnableTryItOutByDefault();
		});

		return app;
	}
}
