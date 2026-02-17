using Articles.API.Authentication;
using Articles.API.Endpoints;
using Articles.API.Extensions;
using Articles.API.Middlewares;
using Articles.Application;
using Articles.Infrastructure;
using Articles.Infrastructure.BackgroundService;
using Articles.Infrastructure.Monitoring;
using Articles.Shared;
using Articles.Storage.Minio;
using Articles.Storage.Postgres;
using Articles.Storage.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.ConfigureMaxRequestBodySize();

configuration.AddJsonFile("rolesOptions.json"); //all roles and their permissions
configuration.AddJsonFile("usageLimitingOptions.json"); //all usage limiting policies

builder.Services
	.AddSwagger()
	.AddRateLimiting()
	.ConfigureCors(configuration);

builder.Services
	.AddApiLogging(configuration, environment)
	.AddApiMetrics(environment)
	.AddApiTracing(configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();

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

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseSwaggerWithUI();

app.UseCors(SecurityExtensions.DefaultCorsPolicy);

await app.InitializeDatabaseAsync();
await app.InitializeFileBucketsAsync();

app.UseMiddleware<GlobalExceptionHandler>()
	.UseMiddleware<AuthenticationMiddleware>();

app.MapPrometheusScrapingEndpoint();

app.MapServiceEndpoints()
	.MapAuthEndpoints()
	.MapFileEndpoints()
	.MapBlogEndpoints()
	.MapArticleEndpoints()
	.MapCommentEndpoints();

app.Run();

public partial class Program;
