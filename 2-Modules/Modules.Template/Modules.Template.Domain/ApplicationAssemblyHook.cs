using System.Reflection;

namespace Modules.Template.Domain;

public class ApplicationAssemblyHook
{
    public static Assembly Assembly => typeof(ApplicationAssemblyHook).Assembly;
}