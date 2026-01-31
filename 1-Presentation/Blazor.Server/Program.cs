using Blazor.Server;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Modules.CodeGenerator.AppServices;
using Modules.Core.AppServices;
using Modules.Core.Blazor;
using Modules.MES.AppServices;
using Modules.My.AppServices;
using Modules.Tasks.AppServices;

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

#region 模块注入(核心注入要放在前面，后面的会覆盖前面的方法)
builder.Services.AddCoreModules(builder.Configuration); //核心模块
builder.Services.AddTaskModules(builder.Configuration); //任务模块
builder.Services.AddMesModules(builder.Configuration); //核心MES模块
builder.Services.AddCodeGeneratorModules(builder.Configuration); //代码生成模块
builder.Services.AddMyModules(builder.Configuration); //我的模块
#endregion

#region API模块控制器
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Core.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.MES.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.My.API._Imports).Assembly); //我的API模块
#endregion

//要放在最后，使用了BuildServiceProvider去调用前面注册的服务
HostService.RegisterHostServices(builder.Services);

var app = builder.Build();

/*开机启动服务例子
var weatherService = app.Services.GetRequiredService<WeatherService>();
await weatherService.InitializeWeatherAsync(
    app.Configuration["WeatherServiceUrl"]);

 //AddHostedService,开机启动后台任务
 */

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(_PageRouteData.AdditionalPages); //注意在这里注册其他页面

#region 模块配置
app.UseCoreAppConfig(); //添加核心模块配置
#endregion

await app.RunAsync().ConfigureAwait(false);