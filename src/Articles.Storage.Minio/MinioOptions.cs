using System.ComponentModel.DataAnnotations;

namespace Articles.Storage.Minio;

public class MinioOptions
{
	[Required]
	public string Endpoint { get; init; } = null!;

	[Required]
	public string AccessKey { get; init; } = null!;

	[Required]
	public string SecretKey { get; init; } = null!;
}
