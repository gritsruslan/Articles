using Articles.Application.Interfaces.Repositories;
using Articles.Shared.Options;
using Articles.Storage.Minio.Initializer;
using Articles.Storage.Minio.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Articles.Storage.Minio;

public static class DependencyInjection
{
	public static IServiceCollection AddMinio(
		this IServiceCollection services)
	{
		services.AddSingleton<IMinioClient, MinioClient>(sp =>
		{
			var minioOptions = sp.GetRequiredService<IOptions<MinioOptions>>().Value;

			return (MinioClient)new MinioClient()
				.WithEndpoint(minioOptions.Endpoint)
				.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
				.WithSSL(false)
				.Build();
		});

		services.AddSingleton<IMinioBucketsInitializer, MinioBucketsInitializer>();

		services.AddScoped<IFileRepository, FileRepository>();

		return services;
	}
}
