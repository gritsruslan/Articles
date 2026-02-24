namespace Articles.Application.UseCases.Files.GetFile;

public sealed record GetFileResponse(Stream FileStream, string ContentType);
