using Articles.Shared.Abstraction.CQRS;

namespace Articles.Application.UseCases.Files.GetFile;

public sealed record GetFileQuery(string FileName) : IQuery<GetFileResponse>;
