using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.CodeGenerator.Template.AppServices.CodeTemplate;

namespace Modules.CodeGenerator.Template.AppServices
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

            services.AddScoped<Domain.CodeTemplate.ICodeTemplateRepository, Repository.CodeTemplate.CodeTemplateRepository>();
            services.AddScoped<ICodeTemplateService, CodeTemplateService>();
            services.AddScoped<ICodeTemplatePageService, CodeTemplatePageService>();
            //tree
            services.AddScoped<ICodeTemplateTreeService, CodeTemplateTreeService>();
            services.AddScoped<Domain.CodeTemplate.ICodeTemplateTreeRepository, Repository.CodeTemplate.CodeTemplateTreeRepository>();
        }
    }
}