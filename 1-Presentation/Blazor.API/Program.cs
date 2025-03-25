using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
#region �������ģ�������
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Template.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.Core.API._Imports).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(Modules.MES.API._Imports).Assembly);
builder.Services.AddOpenApi();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference((x) => //���ʵ�ַ�� scalar/v1
    {
        x.DarkMode = false;
        // x.EnabledClients = [ScalarClient.Libcurl, ScalarClient.HttpClient, ScalarClient.Axios];
    });
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
