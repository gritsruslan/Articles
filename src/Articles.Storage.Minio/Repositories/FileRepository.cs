using Articles.Application.Interfaces.Repositories;
using Minio;
using Minio.DataModel.Args;

namespace Articles.Storage.Minio.Repositories;

internal sealed class FileRepository(IMinioClient minioClient) : IFileRepository
{
	public Task UploadFile(
		string bucketName,
		Stream fileStream,
		string fileName,
		string contentType)
	{
		fileStream.Seek(0, SeekOrigin.Begin);

		return minioClient.PutObjectAsync(
			new PutObjectArgs()
				.WithBucket(bucketName)
				.WithObject(fileName)
				.WithContentType(contentType)
				.WithObjectSize(fileStream.Length)
				.WithStreamData(fileStream)
		);
	}

	public async Task<Stream> GetFile(
		string bucketName,
		string fileName,
		CancellationToken cancellationToken)
	{
		var stream = new MemoryStream();

		await minioClient.GetObjectAsync(
			new GetObjectArgs()
				.WithBucket(bucketName)
				.WithObject(fileName)
				.WithCallbackStream(str => str.CopyTo(stream)), cancellationToken);

		stream.Seek(0, SeekOrigin.Begin);
		return stream;
	}

	public Task DeleteFile(string bucketName, string fileName, CancellationToken cancellationToken) =>
		minioClient.RemoveObjectAsync(
			new RemoveObjectArgs()
				.WithBucket(bucketName)
				.WithObject(fileName), cancellationToken);
}
