namespace Articles.Application.FileUseCases.GetFile;

public sealed record GetFileQuery(string FileName) : IQuery<GetFileResponse>;
