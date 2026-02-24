using Articles.Storage.Minio.Initializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Articles.Infrastructure.ServiceInitializers;

public static class MinioBucketsInitializerExtensions
{
	public static async Task InitializeFileBucketsAsync(this WebApplication app)
	{
		var minioClient = app.Services.GetRequiredService<IMinioClient>();

		var bucketInitializer = app.Services.GetRequiredService<IMinioBucketsInitializer>();

		await bucketInitializer.Initialize(minioClient);
	}
}
