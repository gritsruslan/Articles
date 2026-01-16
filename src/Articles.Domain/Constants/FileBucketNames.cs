using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.Constants;

public static class FileBucketNames
{
	public const string Images = "images";

	public const string Videos = "videos";

	public const string Other = "other";

	public static Result<string> FromContentType(string contentType)
	{
		return contentType switch
		{
			SupportedFileFormats.Jpeg or SupportedFileFormats.Png or SupportedFileFormats.Gif or SupportedFileFormats.Bmp => Images,
			SupportedFileFormats.Mp4 or SupportedFileFormats.WebM or SupportedFileFormats.Mov => Videos,
			SupportedFileFormats.Json or SupportedFileFormats.Xml => Other,
			_ => FileErrors.UnsupportedFileFormat()
		};
	}
}
