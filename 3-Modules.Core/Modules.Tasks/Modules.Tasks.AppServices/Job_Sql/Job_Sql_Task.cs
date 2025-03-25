using Modules.Core.AppServices.Base_solution;
using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks;
using Modules.Tasks.Shared.Constants;
using Quartz;
using SqlSugar;

namespace Modules.Tasks.AppServices.Job_Sql
{
    public class Job_Sql_Task : JobBase, IJob
    {
        IBase_solutionService _SolutionService;
        ISqlSugarClient _DbClient;
        public Job_Sql_Task(ISqlSugarClient DbClient, IBase_solutionService SolutionService, IBase_sys_tasksService tasksQzServices, IBase_sys_tasks_logService qzRunLogService) : base(qzRunLogService, tasksQzServices)
        {
            _DbClient = DbClient;
            _SolutionService = SolutionService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            DateTime dateTime = DateTime.Now;
            var jobKey = context.JobDetail.Key;
            var jobId = jobKey.Name;

            string TaskVM_Json = Convert.ToString(context.JobDetail.JobDataMap["TaskVM_Json"]);
            var TaskVm = System.Text.Json.JsonSerializer.Deserialize<Base_sys_tasksVM>(TaskVM_Json);

            var executeLog = await ExecuteJob(context, async () => await Run(TaskVm));
        }
        //这里
        public async Task<string> Run(Base_sys_tasksVM model)
        {
            var msg = "";
            dynamic exe_ret = null;
            if (model.Task_type == TaskType.Sql语句)
            {
                var solution = await _SolutionService.QueryByIdAsync(model.Task_solution_id);
                if (solution == null)
                {
                    return "解决方案不能为空！";
                }
                this._DbClient = _DbClient.AsTenant().GetConnection(solution.dbid); //切换数据库

                if (model.Sql.ToLower().Contains("update") || model.Sql.ToLower().Contains("delete") || model.Sql.ToLower().Contains("insert"))
                {
                    var ret = await _DbClient.Ado.ExecuteCommandAsync(model.Sql);
                    exe_ret = ret;
                }
                else
                {
                    var ret = await _DbClient.Ado.SqlQueryAsync<dynamic>(model.Sql);
                    exe_ret = ret;
                }

                if (model.Process_task_type != TaskType.无)
                {
                    //执行处理返回结果的程序
                    //msg = await _httpResultProcess.processResult(model, msg);
                }
            }
            return msg;
        }
    }
}
