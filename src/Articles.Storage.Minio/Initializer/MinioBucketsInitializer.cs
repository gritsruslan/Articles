using Articles.Domain.Constants;
using Minio;
using Minio.DataModel.Args;

namespace Articles.Storage.Minio.Initializer;

public sealed class MinioBucketsInitializer : IMinioBucketsInitializer
{
	public async Task Initialize(IMinioClient minioClient)
	{
		string[] buckets = [FileBucketNames.Images, FileBucketNames.Videos, FileBucketNames.Other];

		foreach (var bucket in buckets)
		{
			bool exists = await minioClient.BucketExistsAsync(
				new BucketExistsArgs().WithBucket(bucket));

			if (!exists)
			{
				await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket));
			}
		}
	}
}
