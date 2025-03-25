using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks;
using Modules.Tasks.Shared.Base_sys_tasks_log;
using Modules.Tasks.Shared.Constants;
using Quartz;
using System.Diagnostics;

namespace Modules.Tasks.AppServices
{
    public class JobBase
    {
        IBase_sys_tasks_logService _TaskLogService;
        IBase_sys_tasksService _tasksServices;
        public JobBase(IBase_sys_tasks_logService TaskLogService, IBase_sys_tasksService tasksQzServices)
        {
            _TaskLogService = TaskLogService;
            _tasksServices = tasksQzServices;
        }

        /// <summary>
        /// 执行指定任务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        public async Task<string> ExecuteJob(IJobExecutionContext context, Func<Task<string>> func)
        {
            //int Log_level = 0; //是否记录日志不记录 = 0,记录错误 = 1,记录全部日志 = 2
            //int.TryParse(Convert.ToString(context.JobDetail.JobDataMap["Log_level"]), out Log_level);
            //string Tenant_id = Convert.ToString(context.JobDetail.JobDataMap["Tenant_id"]);
            //string Solution_id = Convert.ToString(context.JobDetail.JobDataMap["Solution_id"]);

            string TaskVM_Json = Convert.ToString(context.JobDetail.JobDataMap["TaskVM_Json"]);
            var TaskVm = System.Text.Json.JsonSerializer.Deserialize<Base_sys_tasksVM>(TaskVM_Json);


            Base_sys_tasks_logVM task_log = new Base_sys_tasks_logVM();
            task_log.Task_id = context.JobDetail.Key.Name;
            task_log.App_id = context.JobDetail.Key.Group;
            task_log.Log_time = DateTime.Now;
            task_log.Log_type = "Normal";
            task_log.Tenant_id = TaskVm.Tenant_id;
            task_log.Solution_id = TaskVm.Task_solution_id;

            string jobHistory = $"【{DateTime.Now}】执行任务【Id：{context.JobDetail.Key.Name}，组别：{context.JobDetail.Key.Group}】";
            string log_msg = "";
            //记录Job时间
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var s = context.Trigger.Key.Name;
                log_msg = await func();//执行任务
                stopwatch.Stop();
                jobHistory += $"，【执行成功】，完成时间：{stopwatch.Elapsed.TotalMilliseconds / 1000}秒";
                //WriteLog(context.Trigger.Key.Name.Replace("-", ""), $"{context.Trigger.Key.Name}定时任务运行一切OK", "任务结束");
                task_log.Log_text =
                        $" 响应结果:[{log_msg}]";
                task_log.Milliseconds = $"{stopwatch.Elapsed.TotalMilliseconds / 1000}秒";
                task_log.Log_msg = log_msg;
                await _tasksServices.AddRun_timeAsync(TaskVm.Id);//累加运行次数
                if (TaskVm.Log_level == TaskLog_Level.记录全部日志) await _TaskLogService.InsertAsync(task_log);
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                //true  是立即重新执行任务 
                e2.RefireImmediately = false;

                stopwatch?.Stop();
                //WriteErrorLog(context.Trigger.Key.Name.Replace("-", ""), $"{context.Trigger.Key.Name}任务运行异常", ex);
                jobHistory += $"，【执行失败】，异常日志：{ex.Message}";
                task_log.Log_text =
                     $"【执行失败】，异常日志：{ex.Message}{ex.StackTrace}";
                task_log.Log_type = "Warn";
                task_log.Milliseconds = $"{stopwatch.Elapsed.TotalMilliseconds / 1000}秒";
                task_log.Log_msg = log_msg;
                if (TaskVm.Log_level > 0) await _TaskLogService.InsertAsync(task_log);
            }
            //Console.Out.WriteLine(jobHistory);
            return jobHistory;
        }
    }



}
