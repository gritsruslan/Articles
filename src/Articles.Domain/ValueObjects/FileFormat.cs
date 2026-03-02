using System.Net.Mime;
using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public sealed record FileFormat(string Extension, string ContentType)
{
	public string Extension { get; set; } = Extension;

	public string ContentType { get; set; } = ContentType;

	public static Result<FileFormat> FromExtension(string extension)
	{
		return extension switch
		{
			".jpeg" => FileFormats.Jpeg,
			".png" => FileFormats.Png,
			".gif" => FileFormats.Gif,
			".bmp" => FileFormats.Bmp,
			".mp4" => FileFormats.Mp4,
			".webm" => FileFormats.WebM,
			".mov" => FileFormats.Mov,
			".json" => FileFormats.Json,
			".xml" => FileFormats.Xml,
			".csv" => FileFormats.Csv,
			_ => FileErrors.UnsupportedFileFormat()
		};
	}

	public static Result<FileFormat> FromContentType(string contentType)
	{
		return contentType switch
		{
			MediaTypeNames.Image.Jpeg => FileFormats.Jpeg,
			MediaTypeNames.Image.Png => FileFormats.Png,
			MediaTypeNames.Image.Gif => FileFormats.Gif,
			MediaTypeNames.Image.Bmp => FileFormats.Bmp,
			MediaTypeNames.Application.Json => FileFormats.Json,
			"video/mp4" => FileFormats.Mp4,
			"video/webm" => FileFormats.WebM,
			"video/mov" => FileFormats.Mov,
			"application/xml" => FileFormats.Xml,
			"application/csv" => FileFormats.Csv,
			_ => FileErrors.UnsupportedFileFormat()
		};
	}
}
