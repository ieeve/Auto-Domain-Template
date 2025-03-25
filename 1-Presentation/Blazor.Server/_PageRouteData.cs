namespace Blazor.Server
{
    public static class _PageRouteData
    {
        //附加Page的路由，使用AdditionalAssemblies 添加路由信息
        public static System.Reflection.Assembly[] AdditionalPages =
        [
            typeof(Modules.Core.Blazor._Imports).Assembly,
            typeof(Modules.Tasks.Blazor._Imports).Assembly,
            typeof(Modules.CodeGenerator.Blazor._Imports).Assembly,
            typeof(Modules.Template.Blazor._Imports).Assembly,
            typeof(Modules.MES.Blazor._Imports).Assembly,
        ];
    }
}
