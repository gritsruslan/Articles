using System.Net.Mime;

namespace Articles.Storage.Minio;

public static class SupportedFormats
{
	// images
	public const string Jpeg = MediaTypeNames.Image.Jpeg;

	public const string Png = MediaTypeNames.Image.Png;

	public const string Gif = MediaTypeNames.Image.Gif;

	public const string Bmp = MediaTypeNames.Image.Bmp;

	// videos
	public const string Mp4 = "video/mp4";

	public const string Mpeg = "video/mpeg";

	public const string WebM = "video/webm";

	public const string Mov = "video/mov";

	// other

	public const string Json = MediaTypeNames.Application.Json;

	public const string Xml = MediaTypeNames.Text.Xml;

	public const string Csv = MediaTypeNames.Text.Csv;
}
