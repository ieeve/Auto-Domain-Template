using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Template.AppServices.CodeTemplate;

namespace Modules.Template.AppServices
{
    public static class AddModulesInjector
    {
        /// <summary>
        /// 注意：需要注册到Program.cs文件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddTemplateModules(this IServiceCollection services, IConfigurationManager configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //注入MediatR(不用单独注入领域事件了)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Domain.ApplicationAssemblyHook).Assembly));

            services.AddScoped<Domain.CodeTemplate.ICodeTemplateRepository, Repository.CodeTemplate.CodeTemplateRepository>();
            services.AddScoped<ICodeTemplateService, CodeTemplateService>();
            services.AddScoped<ICodeTemplatePageService, CodeTemplatePageService>();
            //tree
            services.AddScoped<ICodeTemplateTreeService, CodeTemplateTreeService>();
            services.AddScoped<Domain.CodeTemplate.ICodeTemplateTreeRepository, Repository.CodeTemplate.CodeTemplateTreeRepository>();
        }
    }
}