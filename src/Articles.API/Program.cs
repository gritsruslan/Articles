using Articles.API.Authentication;
using Articles.API.Constants;
using Articles.API.Endpoints;
using Articles.API.Extensions;
using Articles.API.Handlers;
using Articles.API.Middlewares;
using Articles.Application;
using Articles.Infrastructure;
using Articles.Infrastructure.BackgroundService;
using Articles.Infrastructure.Monitoring;
using Articles.Infrastructure.ServiceInitializers;
using Articles.Shared;
using Articles.Storage.Minio;
using Articles.Storage.Postgres;
using Articles.Storage.Redis;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.ConfigureMaxRequestBodySize();

configuration.AddJsonFile("rolesOptions.json"); //all roles and their permissions
configuration.AddJsonFile("usageLimitingOptions.json"); //all usage limiting policies

builder.Services
	.AddSwagger()
	.ConfigureCors(configuration)
	.AddRateLimiting();

builder.Services
	.AddApiLogging(configuration, environment)
	.AddApiMetrics(environment)
	.AddApiTracing(configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();
builder.Services
	.AddTransient<GlobalCommandHandler>()
	.AddTransient<GlobalQueryHandler>();

builder.Services
	.AddHostedService<OutboxProcessorBackgroundService>()
	.AddHostedService<FileCleanerBackgroundService>();

builder.Services
	.AddDefaultServices()
	.ConfigureOptions(configuration);

builder.Services
	.AddApplication()
	.AddInfrastructure(configuration)
	.AddPostgres(configuration)
	.AddRedis(configuration)
	.AddMinio();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>()
	.UseMiddleware<AuthenticationMiddleware>();

if (environment.IsProduction())
{
	app.UseRateLimiter();
	app.UseHttpsRedirection();
}

app.UseSwaggerWithUi();

app.UseCors(CorsConstants.DefaultPolicy);

await app.InitializeDatabaseAsync();
await app.InitializeFileBucketsAsync();

app.MapPrometheusScrapingEndpoint();

app.MapServiceEndpoints()
	.MapAuthEndpoints()
	.MapFileEndpoints()
	.MapBlogEndpoints()
	.MapArticleEndpoints()
	.MapCommentEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions
{
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program;
