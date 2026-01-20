namespace Articles.Application.FileUseCases.GetFile;

public sealed record GetFileResponse(Stream FileStream, string ContentType);
