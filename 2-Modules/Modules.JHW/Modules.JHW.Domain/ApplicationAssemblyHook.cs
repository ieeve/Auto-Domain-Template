using System.Reflection;

namespace Modules.JHW.Domain;

public class ApplicationAssemblyHook
{
    public static Assembly Assembly => typeof(ApplicationAssemblyHook).Assembly;
}