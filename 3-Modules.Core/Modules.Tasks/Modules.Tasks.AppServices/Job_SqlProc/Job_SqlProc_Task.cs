using Modules.Core.AppServices.Base_solution;
using Modules.Tasks.AppServices.Base_sys_tasks;
using Modules.Tasks.AppServices.Base_sys_tasks_log;
using Modules.Tasks.Shared.Base_sys_tasks;
using Modules.Tasks.Shared.Constants;
using Quartz;
using SqlSugar;

namespace Modules.Tasks.AppServices.Job_SqlProc
{
    public class Job_SqlProc_Task : JobBase, IJob
    {
        IBase_solutionService _SolutionService;
        ISqlSugarClient _DbClient;
        public Job_SqlProc_Task(ISqlSugarClient DbClient, IBase_solutionService SolutionService, IBase_sys_tasksService tasksQzServices, IBase_sys_tasks_logService qzRunLogService) : base(qzRunLogService, tasksQzServices)
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
            if (model.Task_type == TaskType.存储过程)
            {
                var solution = await _SolutionService.QueryByIdAsync(model.Task_solution_id);
                this._DbClient = _DbClient.AsTenant().GetConnection(solution.dbid); //切换数据库

                var exe_re = _DbClient.Ado.UseStoredProcedure().SqlQueryAsync<dynamic>(model.Sql_function);

                if (model.Process_task_type != TaskType.无)
                {
                    //执行处理返回结果的程序
                    //msg = await _httpResultProcess.processResult(model, msg);
                }
            }
            return msg;
        }

        public void AddRun_times(Base_sys_tasksVM model)
        {

        }
    }
}
