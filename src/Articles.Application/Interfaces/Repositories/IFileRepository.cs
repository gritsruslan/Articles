namespace Articles.Application.Interfaces.Repositories;

public interface IFileRepository
{
	Task UploadFile(
		string bucketName,
		Stream fileStream,
		string fileName,
		string contentType);

	Task<Stream> GetFile(string bucketName, string fileName, CancellationToken cancellationToken);

	Task DeleteFile(string bucketName, string fileName, CancellationToken cancellationToken);
}
