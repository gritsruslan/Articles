using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class FileErrors
{
	public static Error UnsupportedFileFormat() =>
		new(ErrorType.UnsupportedMediaType, "Unsupported file format");

	public static Error TooLargeFile() =>
		new(ErrorType.EntityTooLarge, "File is too large", "file.too.large");

	public static Error FileNotFound(string fileName) =>
		new(ErrorType.NotFound, "File not found", "file.notfound", "FileName", fileName);
}
