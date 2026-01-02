using System.Reflection;

namespace Articles.Application;

internal static class AssemblyMarker
{
	public static Assembly Assembly => typeof(AssemblyMarker).Assembly;
}
