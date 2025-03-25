using Modules.Core.Shared;
using Modules.Tasks.Shared.Base_sys_tasks;


namespace Modules.Tasks.AppServices.TaskScheduler
{
    public interface ISchedulerCenter
    {

        /// <summary>
        /// 开启任务调度
        /// </summary>
        /// <returns></returns>
        Task<Result> StartScheduleAsync();
        /// <summary>
        /// 停止任务调度
        /// </summary>
        /// <returns></returns>
        Task<Result> StopScheduleAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<Result> AddScheduleJobAsync(Base_sys_tasksVM sysSchedule);
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<Result> StopScheduleJobAsync(Base_sys_tasksVM sysSchedule);
        /// <summary>
        /// 恢复一个任务
        /// </summary>
        /// <param name="sysSchedule"></param>
        /// <returns></returns>
        Task<Result> ResumeJob(Base_sys_tasksVM sysSchedule);

    }
}
