using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
#region 添加其他模块控制器
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Template.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Core.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.MES.API._Imports).Assembly);
builder.Services.AddOpenApi();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference((x) => //访问地址： scalar/v1
    {
        x.DarkMode = false;
        // x.EnabledClients = [ScalarClient.Libcurl, ScalarClient.HttpClient, ScalarClient.Axios];
    });
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
