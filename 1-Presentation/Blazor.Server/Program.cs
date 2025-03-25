using Blazor.Server;
using Modules.CodeGenerator.AppServices;
using Modules.Core.AppServices;
using Modules.Core.Blazor;
using Modules.MES.AppServices;
using Modules.Tasks.AppServices;
using Modules.Template.AppServices;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);

//配置signaIR消息的限制，默认32K，在JS调用截图在时候，大图片不能传递（也可通过分割图片多次传递）
builder.Services.AddRazorComponents(options =>
    options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents().AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // 10MB or use null ，null为不限制大小
    });

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
//添加 Kestrel 配置
//builder.WebHost.UseKestrel(options =>
//{
//    //最大上传
//    options.Limits.MaxRequestBodySize = null;
//    //指定端口，也可以通过appsettings.json配置( //"urls": "http://*:5000", )
//    options.Listen(System.Net.IPAddress.Any, 80);
//});

#region 模块注入(核心注入要放在前面，后面的会覆盖前面的方法)
builder.Services.AddCoreModules(builder.Configuration); //核心模块
builder.Services.AddTaskModules(builder.Configuration); //任务模块
builder.Services.AddMesModules(builder.Configuration); //核心MES模块
builder.Services.AddCodeGeneratorModules(builder.Configuration); //代码生成模块
builder.Services.AddTemplateModules(builder.Configuration); //模板模块

#endregion

#region API模块控制器
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Core.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.MES.API._Imports).Assembly);
#endregion

//要放在最后，使用了BuildServiceProvider去调用前面注册的服务
HostService.RegisterHostServices(builder.Services);

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(_PageRouteData.AdditionalPages); //注意在这里注册其他页面

#region 模块配置
app.UseCoreAppConfig(); //添加核心模块配置
/* MES配置 */
//Modules.MES.Shared.Config.Items_GetId.BUY = "外购";
//Modules.MES.Shared.Config.Items_GetId.MAK = "自制";
#endregion

await app.RunAsync().ConfigureAwait(false);