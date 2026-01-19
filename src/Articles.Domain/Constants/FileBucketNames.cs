using System.Net.Mime;
using Articles.Domain.Errors;
using Articles.Domain.ValueObjects;
using Articles.Shared.Result;

namespace Articles.Domain.Constants;

public static class FileBucketNames
{
	public const string Images = "images";

	public const string Videos = "videos";

	public const string Other = "other";

	public static Result<string> FromFormat(FileFormat format)
	{
		Func<FileFormat, bool> predicate = f => f == format;

		if (SupportedFileFormats.Images().Any(predicate))
		{
			return Images;
		}

		if (SupportedFileFormats.Videos().Any(predicate))
		{
			return Videos;
		}

		if (SupportedFileFormats.Other().Any(predicate))
		{
			return Other;
		}

		return FileErrors.UnsupportedFileFormat();
	}
}
