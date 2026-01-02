using System.Collections.Immutable;
using Articles.Application.Interfaces.Authentication;
using Articles.Domain.Enums;
using Articles.Domain.Identifiers;
using Articles.Domain.Models;
using Articles.Domain.Permissions;
using Articles.Shared.Options;

namespace Articles.Infrastructure.Authorization;

internal sealed class RoleManager : IRoleManager
{
	private RoleManager(Dictionary<int, Role> rolesDictionary)
	{
		RolesDictionary = rolesDictionary;
	}

	private IReadOnlyDictionary<int, Role> RolesDictionary { get; }

	public Role GetRole(RoleId roleId)
	{
		return RolesDictionary[roleId.Value] ??
		       throw new InvalidOperationException($"Role with id {roleId} not found!");
	}

	public Role Guest() => GetRole(RoleId.Create((int)Roles.Guest));

	public static RoleManager ParseFromOptions(RolesOptions options)
	{
		var capacity = options.Roles.Length;
		var rolesDictionary = new Dictionary<int, Role>(capacity);

		foreach (var roleOption in options.Roles)
		{
			var blogPermissions = roleOption.Permissions.BlogPermissions.Select(name =>
				new Permission { Id = (int)Enum.Parse<BlogPermissions>(name), Name = name }).ToImmutableList();

			var articlePermissions = roleOption.Permissions.ArticlePermissions.Select(name =>
				new Permission { Id = (int)Enum.Parse<ArticlePermissions>(name), Name = name }).ToImmutableList();

			var commentPermissions = roleOption.Permissions.CommentPermissions.Select(name =>
				new Permission { Id = (int)Enum.Parse<CommentPermissions>(name), Name = name }).ToImmutableList();

			var adminPermissions = roleOption.Permissions.AdminPermissions.Select(name =>
				new Permission { Id = (int)Enum.Parse<AdminPermissions>(name), Name = name }).ToImmutableList();

			var roleId = RoleId.Create((int)Enum.Parse<Roles>(roleOption.Name));
			var role = new Role
			{
				Id = roleId,
				Name = roleOption.Name,
				Permissions =
				[
					..blogPermissions,
					..articlePermissions,
					..commentPermissions,
					..adminPermissions
				]
			};

			rolesDictionary.Add(roleId.Value, role);
		}

		return new RoleManager(rolesDictionary);
	}
}
