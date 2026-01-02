using System.Reflection;

namespace Articles.Storage.Postgres;

internal static class AssemblyMarker
{
	public static Assembly Assembly => typeof(AssemblyMarker).Assembly;
}
