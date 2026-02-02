using Articles.Application.Authorization;
using Articles.Domain.Permissions;
using Microsoft.AspNetCore.Http;

namespace Articles.Application.FileUseCases.UploadFile;

public sealed record UploadFileCommand(IFormFile File) : ICommand<string>, IAuthorizedCommand
{
	public int RequiredPermissionId => (int) DefaultPermissions.RequireAuthorization;
}
