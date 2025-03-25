using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Modules.Tasks.AppServices
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        //public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        //{
        //    return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        //}

        //public void ReturnJob(IJob job)
        //{

        //}

        ///// <summary>
        ///// 实现接口Job
        ///// </summary>
        ///// <param name="bundle"></param>
        ///// <param name="scheduler"></param>
        ///// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var serviceScope = _serviceProvider.CreateScope();
                var job = serviceScope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
                return job;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

        }
    }
}
