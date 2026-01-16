using System.Net.Mime;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.Constants;

public static class SupportedFileFormats
{
	// images
	public const string Jpeg = MediaTypeNames.Image.Jpeg;

	public const string Png = MediaTypeNames.Image.Png;

	public const string Gif = MediaTypeNames.Image.Gif;

	public const string Bmp = MediaTypeNames.Image.Bmp;

	// videos
	public const string Mp4 = "video/mp4";

	public const string WebM = "video/webm";

	public const string Mov = "video/mov";

	// other

	public const string Json = MediaTypeNames.Application.Json;

	public const string Xml = MediaTypeNames.Text.Xml;

	public const string Csv = MediaTypeNames.Text.Csv;



	public static Result<string> FromExtension(string extension)
	{
		return extension switch
		{
			".jpeg" => Jpeg,
			".png" => Png,
			".gif" => Gif,
			".bmp" => Bmp,
			".mp4" => Mp4,
			".webm" => WebM,
			".mov" => Mov,
			".json" => Json,
			".xml" => Xml,
			".csv" => Csv,
			_ => FileErrors.UnsupportedFileFormat()
		};
	}
}
