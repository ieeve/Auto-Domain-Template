using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.JHW.AppServices
{
    public static class AddModulesInjector
    {
        /// <summary>
        /// ע�⣺��Ҫע�ᵽProgram.cs�ļ�
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddTemplateModulesConfig(this IServiceCollection services, IConfigurationManager configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //ע��MediatR(���õ���ע�������¼���)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Domain.ApplicationAssemblyHook).Assembly));

        }
    }
}