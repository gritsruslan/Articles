using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class FileErrors
{
	public static Error UnsupportedFileFormat() =>
		new(ErrorType.InvalidValue, "Unsupported file format");
}
