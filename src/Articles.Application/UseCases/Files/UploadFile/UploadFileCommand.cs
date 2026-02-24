using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Articles.Shared.Abstraction.CQRS;
using Microsoft.AspNetCore.Http;

namespace Articles.Application.UseCases.Files.UploadFile;

public sealed record UploadFileCommand(IFormFile File) : ICommand<string>, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
