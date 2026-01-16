using Articles.Storage.Minio;
using Articles.Storage.Minio.Initializer;
using Minio;

namespace Articles.API.Extensions;

internal static class MinioBucketsInitializerExtensions
{
	public static async Task InitializeFileBucketsAsync(this WebApplication app)
	{
		var minioClient = app.Services.GetRequiredService<IMinioClient>();

		var bucketInitializer = app.Services.GetRequiredService<IMinioBucketsInitializer>();

		await bucketInitializer.Initialize(minioClient);
	}
}
