using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Articles.Storage.Minio;

public static class DependencyInjection
{
	public static IServiceCollection AddMinio(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var minioOptions = configuration.GetRequiredSection("Minio").Get<MinioOptions>() ??
		                   throw new Exception("Minio configuration is missing");

		services.AddSingleton<IMinioClient, MinioClient>(_ =>
			(MinioClient)new MinioClient()
				.WithEndpoint(minioOptions.Endpoint)
				.WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
				.WithSSL(false)
				.Build());

		return services;
	}
}
