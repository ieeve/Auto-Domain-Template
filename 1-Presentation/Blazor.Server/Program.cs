using Blazor.Server;
using Modules.CodeGenerator.AppServices;
using Modules.Core.AppServices;
using Modules.Core.Blazor;
using Modules.MES.AppServices;
using Modules.Tasks.AppServices;
using Modules.Template.AppServices;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);

//����signaIR��Ϣ�����ƣ�Ĭ��32K����JS���ý�ͼ��ʱ�򣬴�ͼƬ���ܴ��ݣ�Ҳ��ͨ���ָ�ͼƬ��δ��ݣ�
builder.Services.AddRazorComponents(options =>
    options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents().AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // 10MB or use null ��nullΪ�����ƴ�С
    });

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
//��� Kestrel ����
//builder.WebHost.UseKestrel(options =>
//{
//    //����ϴ�
//    options.Limits.MaxRequestBodySize = null;
//    //ָ���˿ڣ�Ҳ����ͨ��appsettings.json����( //"urls": "http://*:5000", )
//    options.Listen(System.Net.IPAddress.Any, 80);
//});

#region ģ��ע��(����ע��Ҫ����ǰ�棬����ĻḲ��ǰ��ķ���)
builder.Services.AddCoreModules(builder.Configuration); //����ģ��
builder.Services.AddTaskModules(builder.Configuration); //����ģ��
builder.Services.AddMesModules(builder.Configuration); //����MESģ��
builder.Services.AddCodeGeneratorModules(builder.Configuration); //��������ģ��
builder.Services.AddTemplateModules(builder.Configuration); //ģ��ģ��

#endregion

#region APIģ�������
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Core.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.MES.API._Imports).Assembly);
#endregion

//Ҫ�������ʹ����BuildServiceProviderȥ����ǰ��ע��ķ���
HostService.RegisterHostServices(builder.Services);

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(_PageRouteData.AdditionalPages); //ע��������ע������ҳ��

#region ģ������
app.UseCoreAppConfig(); //��Ӻ���ģ������
/* MES���� */
//Modules.MES.Shared.Config.Items_GetId.BUY = "�⹺";
//Modules.MES.Shared.Config.Items_GetId.MAK = "����";
#endregion

await app.RunAsync().ConfigureAwait(false);