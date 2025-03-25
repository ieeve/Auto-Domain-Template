using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Quartz;

namespace Modules.Tasks.AppServices.Job_Demo1
{
    public class Job_Demo1_Quartz : JobBase, IJob
    {
        public Job_Demo1_Quartz(IBase_sys_tasksService tasksQzServices, IBase_sys_tasks_logService qzRunLogService) : base(qzRunLogService, tasksQzServices)
        {

        }
        public async Task Execute(IJobExecutionContext context)
        {
            // await Console.Out.WriteLineAsync("开始" + 1);
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;
            var executeLog = await ExecuteJob(context, async () => await Run(context, jobId));

            // 也可以通过数据库配置，获取传递过来的参数
            JobDataMap data = context.JobDetail.JobDataMap;
        }
        public async Task<string> Run(IJobExecutionContext context, string jobid)
        {
            await Console.Out.WriteLineAsync("你好!我是测试Demo1");
            return "你好!我是测试Demo1";
        }
    }

}
