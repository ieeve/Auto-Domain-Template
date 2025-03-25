using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Tasks.AppServices.Job_Demo1;
using Modules.Tasks.AppServices.Job_Http;
using Modules.Tasks.AppServices.Job_Sql;
using Modules.Tasks.AppServices.Job_SqlProc;
using Modules.Tasks.AppServices.TaskScheduler;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Modules.Tasks.AppServices
{
    public static class AddModulesInjector
    {
        /// <summary>
        /// 注意：需要注册到Program.cs文件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddTaskModules(this IServiceCollection services, IConfigurationManager configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            //注入MediatR(不用单独注入领域事件了)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Domain.ApplicationAssemblyHook).Assembly));

            //启动就运行的服务(任务模块)
            services.AddHostedService<QuartzHostedService>();

            //Base服务
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerCenter, SchedulerCenter>();

            //Base_sys_tasks 
            services.AddScoped<Domain.Base_sys_tasks.IBase_sys_tasksRepository, Repository.Base_sys_tasks.Base_sys_tasksRepository>();
            services.AddScoped<AppServices.Base_sys_tasks.IBase_sys_tasksService, AppServices.Base_sys_tasks.Base_sys_tasksService>();
            services.AddScoped<AppServices.Base_sys_tasks.IBase_sys_tasksPageService, AppServices.Base_sys_tasks.Base_sys_tasksPageService>();
            //Base_sys_tasks_log 
            services.AddScoped<Domain.Base_sys_tasks_log.IBase_sys_tasks_logRepository, Repository.Base_sys_tasks_log.Base_sys_tasks_logRepository>();
            services.AddScoped<AppServices.Base_sys_tasks_log.IBase_sys_tasks_logService, AppServices.Base_sys_tasks_log.Base_sys_tasks_logService>();
            services.AddScoped<AppServices.Base_sys_tasks_log.IBase_sys_tasks_logPageService, AppServices.Base_sys_tasks_log.Base_sys_tasks_logPageService>();

            #region 任务
            services.AddTransient<Job_HttpApi_Task>();//HTTP任务
            services.AddTransient<Job_Sql_Task>();
            services.AddTransient<Job_SqlProc_Task>();
            //处理http任务的结果
            services.AddScoped<httpResultProcessService>();
            //Demo任务
            services.AddTransient<Job_Demo1_Quartz>();
            #endregion
        }
    }
}