using Modules.Core.Shared;
using Modules.Tasks.AppServices.Job_Http;
using Modules.Tasks.AppServices.Job_Sql;
using Modules.Tasks.AppServices.Job_SqlProc;
using Modules.Tasks.Shared.Base_sys_tasks;
using Modules.Tasks.Shared.Constants;
using Quartz;
using Quartz.Spi;
using System.Reflection;

namespace Modules.Tasks.AppServices.TaskScheduler
{
    /// <summary>
    /// 任务调度管理中心
    /// </summary>
    public class SchedulerCenter : ISchedulerCenter
    {
        private IScheduler _scheduler;

        private readonly IJobFactory _iocjobFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        public SchedulerCenter(IJobFactory jobFactory, ISchedulerFactory schedulerFactory)
        {
            _iocjobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            _schedulerFactory = schedulerFactory;
            _scheduler = GetSchedulerAsync().Result;

        }
        private async Task<IScheduler> GetSchedulerAsync()
        {
            if (_scheduler != null)
                return _scheduler;
            else
            {
                // 从Factory中获取Scheduler实例
                return _scheduler = await _schedulerFactory.GetScheduler();
            }
        }

        #region _scheduler层级
        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<Result> StartScheduleAsync()
        {
            try
            {
                _scheduler.JobFactory = _iocjobFactory;
                if (!_scheduler.IsStarted)
                {
                    //等待任务运行完成
                    await _scheduler.Start();
                    await Console.Out.WriteLineAsync("任务调度开启！");
                    return Result.Success("任务调度开启成功");
                }
                else
                {
                    return Result.Info("任务调度已经开启");

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        public async Task<Result> StopScheduleAsync()
        {
            try
            {
                if (!_scheduler.IsShutdown)
                {
                    //await Task.Delay(30);
                    //等待任务运行完成
                    await _scheduler.Shutdown();
                    await Console.Out.WriteLineAsync("任务调度停止！");
                    return Result.Success("任务调度停止成功");
                }
                else
                {
                    return Result.Info("任务调度已经停止");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        /// <summary>
        /// 添加一个计划任务（映射程序集指定IJob实现类）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<Result> AddScheduleJobAsync(Base_sys_tasksVM tasksQz)
        {

            if (tasksQz != null)
            {
                try
                {
                    JobKey jobKey = new JobKey(tasksQz.Id.ToString());
                    if (await _scheduler.CheckExists(jobKey))
                    {
                        return Result.Info($"该任务计划已经在执行:【{tasksQz.Name}】,请勿重复启动！");
                    }

                    #region 通过反射获取程序集类型和类   
                    var Assembly_name = "Modules.Tasks.AppServices";  //不给用户设置权限，使用当然任务所在项目名
                    //tasksQz.Class_name.Replace(Assembly_name + ".", "");
                    Assembly assembly = Assembly.Load(new AssemblyName(Assembly_name));
                    var test = assembly.GetType(Assembly_name + ".Job_Sql.Job_Sql_Task");
                    Type? jobType = null;
                    if (tasksQz.Task_type == TaskType.程序集) jobType = assembly.GetType(Assembly_name + "." + tasksQz.Class_name);
                    if (tasksQz.Task_type == TaskType.WebApi) jobType = typeof(Job_HttpApi_Task);
                    if (tasksQz.Task_type == TaskType.存储过程) jobType = typeof(Job_SqlProc_Task);
                    if (tasksQz.Task_type == TaskType.Sql语句) jobType = typeof(Job_Sql_Task);
                    #endregion
                    //判断任务调度是否开启
                    if (!_scheduler.IsStarted)
                    {
                        await StartScheduleAsync();
                    }
                    #region 泛型传递
                    //传入反射出来的执行程序集
                    //IJobDetail job = new JobDetailImpl(tasksQz.Id.ToString(), tasksQz.AppId, jobType);
                    //job.JobDataMap.Add("JobParam", tasksQz.JobParams);

                    IJobDetail jobdetail = JobBuilder.Create(jobType)
                          //.UsingJobData("Log_level", (int)tasksQz.Log_level) //可附加一些数据,这里附加记录日志的级别
                          // .UsingJobData("Tenant_id", tasksQz.Tenant_id)
                          //  .UsingJobData("Solution_id", tasksQz.Task_solution_id)
                          .UsingJobData("TaskVM_Json", System.Text.Json.JsonSerializer.Serialize(tasksQz)) //附加了整个task模型
                        .WithIdentity(tasksQz.Id.ToString()).Build();

                    //IJobDetail job = JobBuilder.Create<T>()
                    //    .WithIdentity(sysSchedule.Name, sysSchedule.JobGroup)
                    //    .Build();
                    #endregion
                    ITrigger trigger;
                    if (tasksQz.Cron != null && CronExpression.IsValidExpression(tasksQz.Cron) && tasksQz.Is_cron)
                    {
                        trigger = CreateCronTrigger(tasksQz);
                    }
                    else
                    {
                        //tasksQz.IntervalSecond = 5;
                        trigger = CreateSimpleTrigger(tasksQz);
                    }

                    // 告诉Quartz使用我们的触发器来安排作业
                    await _scheduler.ScheduleJob(jobdetail, trigger);
                    return Result.Success($"启动任务:【{tasksQz.Name}】成功");
                }
                catch (Exception ex)
                {
                    return Result.Error($"任务计划异常:【{ex.Message}】");
                }
            }
            else
            {
                return Result.Error($"任务计划不存在:【{tasksQz?.Name}】");
            }
        }

        /// <summary>
        /// 暂停一个指定的计划任务
        /// </summary>
        /// <returns></returns>
        public async Task<Result> StopScheduleJobAsync(Base_sys_tasksVM qzModel)
        {
            try
            {
                JobKey jobKey = new JobKey(qzModel.Id.ToString());
                if (!await _scheduler.CheckExists(jobKey))
                {
                    //return Result.Info($"未找到要停止的任务:【{qzModel.Name}】");
                    //改成返回 Success，未找到认为已停止
                    return Result.Success($"未找到要停止的任务:【{qzModel.Name}】");
                }
                else
                {
                    //暂停还在_scheduler中，这边直接移除
                    await _scheduler.PauseJob(jobKey);
                    var res = await _scheduler.DeleteJob(jobKey);
                    if (!res)
                    {
                        return Result.Error($"停止任务:【{qzModel.Name}】失败");
                    }
                    return Result.Success($"停止任务:【{qzModel.Name}】成功");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 恢复指定的计划任务
        /// </summary>
        /// <param name="tasksQz"></param>
        /// <returns></returns>
        public async Task<Result> ResumeJob(Base_sys_tasksVM qzModel)
        {
            try
            {
                JobKey jobKey = new JobKey(qzModel.Id.ToString());
                if (!await _scheduler.CheckExists(jobKey))
                {
                    return Result.Info($"未找到要重新的任务:【{qzModel.Name}】,请先选择添加计划！");
                }
                ITrigger trigger;
                if (qzModel.Cron != null && CronExpression.IsValidExpression(qzModel.Cron) && qzModel.Is_cron)
                {
                    trigger = CreateCronTrigger(qzModel);
                }
                else
                {
                    trigger = CreateSimpleTrigger(qzModel);
                }

                TriggerKey triggerKey = new TriggerKey(qzModel.Id.ToString());
                await _scheduler.RescheduleJob(triggerKey, trigger);
                return Result.Success($"恢复计划任务:【{qzModel.Name}】成功");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region 创建触发器帮助方法

        /// <summary>
        /// 创建SimpleTrigger触发器（简单触发器）
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <param name="starRunTime"></param>
        /// <param name="endRunTime"></param>
        /// <returns></returns>
        private ITrigger CreateSimpleTrigger(Base_sys_tasksVM qzModel)
        {
            /*
            if (qzModel.RunTimes > 0)
            {
                ITrigger trigger = TriggerBuilder.Create()
                //.StartNow()
                .WithIdentity(qzModel.Id.ToString(), qzModel.AppId)
                .WithSimpleSchedule(x =>
                x.WithIntervalInSeconds(qzModel.IntervalSecond)
                .WithRepeatCount(qzModel.RunTimes))//指定了执行次数
                .ForJob(qzModel.Id.ToString(), qzModel.AppId).Build();
                return trigger;
            }
            else
            {
                ITrigger trigger = TriggerBuilder.Create()
                //.StartNow()
                .WithIdentity(qzModel.Id.ToString(), qzModel.AppId)
                .WithSimpleSchedule(x =>
                x.WithIntervalInSeconds(qzModel.IntervalSecond)
                .RepeatForever()).ForJob(qzModel.Id.ToString(), qzModel.AppId).Build();
                return trigger;
            }
            */

            ITrigger trigger = TriggerBuilder.Create()
               //.StartNow() // 触发作业立即运行，然后每10秒重复一次，无限循环
               .WithIdentity(qzModel.Id.ToString())
               .WithSimpleSchedule(x =>
               x.WithIntervalInSeconds(qzModel.Interval_second)
               .RepeatForever()).ForJob(qzModel.Id.ToString()).Build();
            return trigger;

        }



        /// <summary>
        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private ITrigger CreateCronTrigger(Base_sys_tasksVM qzModel)
        {
            // 作业触发器
            return TriggerBuilder.Create()
                   .WithIdentity(qzModel.Id.ToString())
                   //.StartAt(qzModel.BeginTime.Value)//开始时间
                   //.EndAt(qzModel.EndTime.Value)//结束数据
                   .WithCronSchedule(qzModel.Cron)//指定cron表达式
                                                  //.StartNow()
                   .ForJob(qzModel.Id.ToString())//作业名称
                   .Build();
        }
        #endregion

    }
}
