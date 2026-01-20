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
			".jpeg" => SupportedFileFormats.Jpeg,
			".png" => SupportedFileFormats.Png,
			".gif" => SupportedFileFormats.Gif,
			".bmp" => SupportedFileFormats.Bmp,
			".mp4" => SupportedFileFormats.Mp4,
			".webm" => SupportedFileFormats.WebM,
			".mov" => SupportedFileFormats.Mov,
			".json" => SupportedFileFormats.Json,
			".xml" => SupportedFileFormats.Xml,
			".csv" => SupportedFileFormats.Csv,
			_ => FileErrors.UnsupportedFileFormat()
		};
	}

	public static Result<FileFormat> FromContentType(string contentType)
	{
		return contentType switch
		{
			MediaTypeNames.Image.Jpeg => SupportedFileFormats.Jpeg,
			MediaTypeNames.Image.Png => SupportedFileFormats.Png,
			MediaTypeNames.Image.Gif => SupportedFileFormats.Gif,
			MediaTypeNames.Image.Bmp => SupportedFileFormats.Bmp,
			"video/mp4" => SupportedFileFormats.Mp4,
			"video/webm" => SupportedFileFormats.WebM,
			"video/mov" => SupportedFileFormats.Mov,
			MediaTypeNames.Application.Json => SupportedFileFormats.Json,
			"application/xml" => SupportedFileFormats.Xml,
			"application/csv" => SupportedFileFormats.Csv,
			_ => FileErrors.UnsupportedFileFormat()
		};
	}
}
