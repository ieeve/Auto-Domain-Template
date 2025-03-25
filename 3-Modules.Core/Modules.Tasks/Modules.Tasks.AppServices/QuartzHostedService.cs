using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Core.Shared;
using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.TaskScheduler;

namespace Modules.Tasks.AppServices
{
    /// <summary>
    /// 使用AddHostedService
    /// 系统运行起来后，它会自动调用StartAsync()方法
    /// </summary>
    public class QuartzHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private IBase_sys_tasksService tasksService;
        private ISchedulerCenter schedulerCenter;
        IHostEnvironment env; //注入运行环境，
        public QuartzHostedService(IServiceProvider serviceProvider, ISchedulerCenter schedulerCenter, IHostEnvironment env)
        {
            this._serviceProvider = serviceProvider;
            this.schedulerCenter = schedulerCenter;
            this.env = env;
            //在单例类中使用scoop项目
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                this.tasksService = scope.ServiceProvider.GetRequiredService<IBase_sys_tasksService>();
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (env.IsDevelopment()) return; //不执行计划任务(开发环境)
            try
            {
                var allQzServices = await tasksService.GetAllStartAsync();
                foreach (var item in allQzServices)
                {
                    var ResuleModel = await schedulerCenter.AddScheduleJobAsync(item);
                    if (ResuleModel.Code == ResultType.Success)
                    {
                        Console.WriteLine($"QuartzNetJob{item.Name}启动成功！");
                    }
                    else
                    {
                        //设置启动失败的程序为false
                        item.Is_start = false;
                        await tasksService.UpdateAsync(item);
                        Console.WriteLine($"QuartzNetJob{item.Name}启动失败！错误信息：{ResuleModel.Msg}");
                    }
                }
            }
            catch (Exception e)
            {
                //log.Error($"An error was reported when starting the job service.\n{e.Message}");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await schedulerCenter.StopScheduleAsync();
        }
        public void dowork(object state)
        {
            exec();
        }
        /// <summary>
        /// 执行代码块
        /// </summary>
        public void exec()
        {
            Console.WriteLine("第{0}次执行");
        }
        public void Dispose()
        {

        }
    }
}
