using System.Reflection;

namespace Modules.Tasks.Domain;

public class ApplicationAssemblyHook
{
    public static Assembly Assembly => typeof(ApplicationAssemblyHook).Assembly;
}