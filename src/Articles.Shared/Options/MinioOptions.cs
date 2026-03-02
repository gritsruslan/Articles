using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public class MinioOptions
{
	[Required]
	public string Endpoint { get; set; } = null!;

	[Required]
	public string AccessKey { get; set; } = null!;

	[Required]
	public string SecretKey { get; set; } = null!;
}
