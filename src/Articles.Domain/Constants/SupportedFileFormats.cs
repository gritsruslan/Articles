using System.Net.Mime;
using Articles.Domain.ValueObjects;

namespace Articles.Domain.Constants;

public static class SupportedFileFormats
{
	//images
	public static readonly FileFormat Jpeg = new(".jpeg", MediaTypeNames.Image.Jpeg);
	public static readonly FileFormat Png = new(".png", MediaTypeNames.Image.Png);
	public static readonly FileFormat Gif = new(".gif", MediaTypeNames.Image.Gif);
	public static readonly FileFormat Bmp = new(".bmp", MediaTypeNames.Image.Bmp);

	// videos
	public static readonly FileFormat Mp4 = new(".mp4", "video/mp4");
	public static readonly FileFormat WebM = new(".webm", "video/webm");
	public static readonly FileFormat Mov = new(".mov", "video/mov");

	// other
	public static readonly FileFormat Json = new(".json", MediaTypeNames.Application.Json);
	public static readonly FileFormat Xml = new(".xml", "application/xml");
	public static readonly FileFormat Csv = new(".csv", "application/csv");

	public const long MaxFileSize = 100_000_000; // 100MB

	public const long MaxFileSizeMb = MaxFileSize / 1_000_000;

	public const int ContentTypeMaxLength = 255;

	public static IEnumerable<FileFormat> Images()
	{
		yield return Jpeg;
		yield return Png;
		yield return Gif;
		yield return Bmp;
	}

	public static IEnumerable<FileFormat> Videos()
	{
		yield return Mp4;
		yield return WebM;
		yield return Mov;
	}

	public static IEnumerable<FileFormat> Other()
	{
		yield return Json;
		yield return Xml;
		yield return Csv;
	}
}
