using Minio;

namespace Articles.Storage.Minio.Initializer;

public interface IMinioBucketsInitializer
{
	Task Initialize(IMinioClient minioClient);
}
